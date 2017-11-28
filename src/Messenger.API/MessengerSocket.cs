using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using Messenger.API.IO;
using Messenger.API.Package;
using Messenger.API.Package.Request;
using Messenger.API.Package.Response;
using Messenger.API.Utility;

namespace Messenger.API
{
    public sealed class MessengerSocket
    {
        #region Internal Data

        private readonly TcpClient tcpClient;
        private readonly MessengerReader reader;
        private readonly StreamWriter writer;
        //Este muito possivelmente poderá ser o segredo do meu sucesso!
        private readonly Encoding encoding = new UTF8Encoding(false, false);
        private int timeout;

        #endregion

        #region .ctor

        // Main workload
        public MessengerSocket(string hostname, int port, int timeout)
        {
            this.Timeout = timeout;

            this.tcpClient = new TcpClient(hostname, port);

            this.writer = new StreamWriter(this.tcpClient.GetStream(), this.encoding);
            this.reader = new MessengerReader(this.tcpClient.GetStream());
            this.reader.MessageReceived += this.MessageReceived;
            this.reader.StartListening();

            if (Debugger.IsAttached)
            {
                //Console.OutputEncoding = encoding;
            }

        }

        public MessengerSocket(string hostname, int port)
            : this(hostname, port, DefaultTimeout)
        { }

        // Work Around a Bug!
        public MessengerSocket(IPEndPoint endPoint, int timeout)
            : this(endPoint.Address.ToString(), endPoint.Port, timeout)
        { }

        public MessengerSocket(IPEndPoint endPoint)
            : this(endPoint, DefaultTimeout)
        { }

        // Almost Default constructor
        public MessengerSocket(int timeout)
            : this("messenger.hotmail.com", 1863, timeout)
        { }

        // Default constructor
        public MessengerSocket()
            : this("messenger.hotmail.com", 1863, DefaultTimeout)
        { }

        #endregion

        #region Properties

        public bool AutoFlush
        {
            get { return this.writer.AutoFlush; }
            set { this.writer.AutoFlush = value; }
        }

        public bool Connected
        {
            get
            {
                return this.tcpClient.Connected;
            }
        }

        public int Timeout
        {
            get
            {
                return this.timeout;
            }
            set
            {
                if (value <= 0)
                {
                    throw new ArgumentOutOfRangeException("value");
                }
                this.timeout = value;
            }
        }

        public Encoding Encoding
        {
            get
            {
                return this.encoding;
            }
        }

        internal MessengerReader Reader
        {
            get
            {
                return this.reader;
            }
        }

        public static int DefaultTimeout
        {
            get { return 5000; }
        }

        #endregion

        #region General Purpose

        public void Flush()
        {
            this.writer.Flush();
        }

        public NetworkStream GetStream()
        {
            return this.tcpClient.GetStream();
        }

        public void Dispose()
        {
            if (this.tcpClient != null)
            {
                this.tcpClient.Close();
            }
            if (this.Reader != null)
            {
                this.Reader.Dispose();
            }
        }

        private void MessageReceived(object sender, MessageEventArgs e)
        {
            PrintReceivedCommand(e.Package);
        }

        #endregion

        #region Debug

        [Conditional("DEBUG")]
        private static void PrintSentCommand(PackageRequest package)
        {
            lock (Console.Out)
            {
                ConsoleColor backup = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.Red;
                const int length = 32;
                Console.WriteLine(new string('>', length));
                Console.WriteLine(package.ToString());
                Console.WriteLine(new string('-', length));
                Console.ForegroundColor = backup;
            }
        }

        [Conditional("DEBUG")]
        private static void PrintReceivedCommand(PackageResponse response)
        {
            if (response is DeclarationResponse || response is InboxMessageResponse || response is ProfileInformationResponse)
            {
                return;
            }

            lock (Console.Out)
            {
                ConsoleColor backup = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.Yellow;
                const int length = 32;
                Console.WriteLine(new string('<', length));
                Console.WriteLine(response);
                Console.WriteLine(new string('-', length));
                Console.ForegroundColor = backup;
            }

        }

        #endregion

        #region Send

        #region Send Range

        public IEnumerable<int> SendRange(IEnumerable<PackageRequest> request)
        {
            List<int> list = new List<int>();
            foreach (PackageRequest item in request)
            {
                list.Add(this.Send(item));
            }
            return list;
        }

        #endregion

        #region Send

        public int Send(PackageRequest request)
        {
            PrintSentCommand(request);
            if (request.IsPayload)
            {
                this.writer.Write(request.ToString());
            }
            else
            {
                this.writer.WriteLine(request.ToString());
            }
            return request.TransactionID;
        }

        #endregion

        #region Send And Block

        public PackageResponse SendAndBlock(PackageRequest request, int timeoutP)
        {
            Dictionary<int, PackageTransaction> response =  this.SendRangeAndBlock(new PackageRequest[] { request }, timeoutP);

            if(response == null || !response.ContainsKey(request.TransactionID))
            {
                return null;
            }

            return response[request.TransactionID].Response;
        }

        public PackageResponse SendAndBlock(PackageRequest request)
        {
            return this.SendAndBlock(request, this.Timeout);
        }

        public PackageResponse SendAndBlock<T>(PackageRequest request, int timeoutP)
        {
            Dictionary<int, PackageTransaction> response = this.SendRangeAndBlock<T>(new PackageRequest[] { request }, timeoutP);

            if (response == null || !response.ContainsKey(request.TransactionID))
            {
                return null;
            }

            return response[request.TransactionID].Response;
        }

        public PackageResponse SendAndBlock<T>(PackageRequest request)
        {
            return this.SendAndBlock<T>(request, this.Timeout);
        }

        #endregion

        #region Send Range And Block

        public Dictionary<int, PackageTransaction> SendRangeAndBlock(IEnumerable<PackageRequest> request, int timeoutP)
        {
            if (timeoutP < 0)
            {
                throw new ArgumentOutOfRangeException("timeoutP", "parameter is less than 0");
            }

            //Enviar comandos!
            List<int> sent = new List<int>(this.SendRange(request));
            //Guardar as respostas aqui!
            Dictionary<int, PackageResponse> responses = new Dictionary<int, PackageResponse>(sent.Count);

            #region Callback/Listener

            EventHandler<MessageEventArgs> method = null;
            //MessengerReader.MessageEventHandler method = null;
            method = delegate(object o, MessageEventArgs e)
                         {
                             lock (sent)
                             {
                                 if (sent.Contains(e.Package.TransactionID))
                                 {
                                     lock (responses)
                                     {
                                         if (!responses.ContainsKey(e.Package.TransactionID))
                                         {
                                             responses.Add(e.Package.TransactionID, e.Package);
                                             if (responses.Count == request.Count())
                                             {
                                                 lock (this.Reader)
                                                 {
                                                     this.Reader.MessageReceived -= method;
                                                     timeoutP = 0;
                                                 }
                                             }
                                         }
                                     }
                                 }
                             }

                         };

            #endregion

            this.Reader.MessageReceived += method;
            this.Flush();

            //Agora vem a parte mesmo lixada!
            //Queremos bloquear a execução do programa até que tenhamos o output desejado!
            //Ou até que ocorra timeoutP!

            #region Block

            //Por enquanto vamos usar esta técnica!
            //Que é uma grande MERDA!
            //Mas enquanto eu não aprendo uma melhor...
            int start = Environment.TickCount;
            while ((Environment.TickCount - start) < timeoutP)
            {
                Thread.Sleep(100);
            }

            this.Reader.MessageReceived -= method;

            #endregion

            //Vamos associar a cada envio uma resposta!
            Dictionary<int, PackageTransaction> result = new Dictionary<int, PackageTransaction>();
            int i = 0;
            foreach (PackageRequest item in request)
            {
                if (!responses.ContainsKey(sent[i])) continue;
                PackageResponse rsp = responses[sent[i]];
                if (rsp == null) continue;
                result.Add(sent[i++], new PackageTransaction(item, rsp));
            }

            return result;

        }

        public Dictionary<int, PackageTransaction> SendRangeAndBlock(IEnumerable<PackageRequest> request)
        {
            return this.SendRangeAndBlock(request, this.Timeout);
        }

        private Dictionary<int, PackageTransaction> SendRangeAndBlock<T>(IEnumerable<PackageRequest> request, int timeoutP)
        {
            if (timeoutP < 0)
            {
                throw new ArgumentOutOfRangeException("timeoutP", "parameter is less than 0");
            }

            //Enviar comandos!
            List<int> sent = new List<int>(this.SendRange(request));
            //Guardar as respostas aqui!
            Dictionary<int, PackageResponse> responses = new Dictionary<int, PackageResponse>(sent.Count);

            #region Callback/Listener

            EventHandler<MessageEventArgs> method = null;
            //MessengerReader.MessageEventHandler method = null;
            method = delegate(object o, MessageEventArgs e)
                         {
                             lock (sent)
                             {
                                 if (sent.Contains(e.Package.TransactionID))
                                 {
                                     lock (responses)
                                     {
                                         if (!responses.ContainsKey(e.Package.TransactionID) && e.Package is T)
                                         {
                                             responses.Add(e.Package.TransactionID, e.Package);
                                             if (responses.Count == request.Count())
                                             {
                                                 lock (this.Reader)
                                                 {
                                                     this.Reader.MessageReceived -= method;
                                                     timeoutP = 0;
                                                 }
                                             }
                                         }
                                     }
                                 }
                             }

                         };

            #endregion

            this.Reader.MessageReceived += method;
            this.Flush();

            //Agora vem a parte mesmo lixada!
            //Queremos bloquear a execução do programa até que tenhamos o output desejado!
            //Ou até que ocorra timeoutP!

            #region Block

            //Por enquanto vamos usar esta técnica!
            //Que é uma grande MERDA!
            //Mas enquanto eu não aprendo uma melhor...
            int start = Environment.TickCount;
            while ((Environment.TickCount - start) < timeoutP)
            {
                Thread.Sleep(100);
            }

            this.Reader.MessageReceived -= method;

            #endregion

            //Vamos associar a cada envio uma resposta!
            Dictionary<int, PackageTransaction> result = new Dictionary<int, PackageTransaction>();
            int i = 0;
            foreach (PackageRequest item in request)
            {
                if (!responses.ContainsKey(sent[i])) continue;
                PackageResponse rsp = responses[sent[i]];
                if (rsp == null) continue;
                result.Add(sent[i++], new PackageTransaction(item, rsp));
            }

            return result;

        }

        //private Dictionary<int, PackageTransaction> SendRangeAndBlock<T>(IEnumerable<PackageRequest> request)
        //{
        //    return SendRangeAndBlock<T>(request, this.Timeout);
        //}

        #endregion

        #endregion

        #region Autentication Helpers

        private static byte[] Join(byte[] b1, byte[] b2)
        {
            //List<byte> list = new List<byte>(b1);
            //list.AddRange(b2);

            //return list.ToArray();

            byte[] ret = new byte[b1.Length + b2.Length];

            Array.Copy(b1, ret, b1.Length);
            Array.Copy(b2, 0, ret, b1.Length, b2.Length);

            return ret;
        }

        private static byte[] DeriveMBIKey(byte[] key, string magic)
        {
            HMACSHA1 sha = new HMACSHA1 {Key = key};

            byte[] magicb = Encoding.UTF8.GetBytes(magic);

            byte[] hash1 = sha.ComputeHash(magicb);
            byte[] hash2 = sha.ComputeHash(Join(hash1, magicb));
            byte[] hash3 = sha.ComputeHash(hash1);
            byte[] hash4 = sha.ComputeHash(Join(hash3, magicb));

            byte[] ret = new byte[hash2.Length + 4];
            Array.Copy(hash2, ret, hash2.Length);
            Array.Copy(hash4, 0, ret, hash2.Length, 4);

            return ret;
        }

        public static string CalculateMBIResponse(string key, string nonce)
        {
            byte[] key1 = Convert.FromBase64String(key);
            byte[] key2 = DeriveMBIKey(key1, "WS-SecureConversationSESSION KEY HASH");
            byte[] key3 = DeriveMBIKey(key1, "WS-SecureConversationSESSION KEY ENCRYPTION");

            HMACSHA1 sha = new HMACSHA1 {Key = key2};

            byte[] hash = sha.ComputeHash(Encoding.UTF8.GetBytes(nonce));

            byte[] iv = new byte[8];
            new Random().NextBytes(iv);

            TripleDESCryptoServiceProvider des3 = new TripleDESCryptoServiceProvider
                                                      {
                                                          Key = key3,
                                                          Mode = CipherMode.CBC,
                                                          IV = iv
                                                      };

            ICryptoTransform akm = des3.CreateEncryptor();
            byte[] input = Encoding.UTF8.GetBytes(nonce + (char)8 + (char)8 + (char)8 + (char)8 + (char)8 + (char)8 + (char)8 + (char)8);
            byte[] output = new byte[72];
            akm.TransformBlock(input, 0, input.Length, output, 0);

            byte[] blob = new byte[28 + iv.Length + hash.Length + output.Length];
            MemoryStream stream = new MemoryStream(blob);
            BinaryWriter writerB = new BinaryWriter(stream);

            writerB.Write(BitUtility.FromInt32(28, true));
            writerB.Write(BitUtility.FromInt32(1, true));              //CRYPT_MODE_CBC 
            writerB.Write(BitUtility.FromInt32(26115, true));          //CALC_3DES 
            writerB.Write(BitUtility.FromInt32(32772, true));          //CALG_SHA1 
            writerB.Write(BitUtility.FromInt32(iv.Length, true));
            writerB.Write(BitUtility.FromInt32(hash.Length, true));
            writerB.Write(BitUtility.FromInt32(output.Length, true));
            writerB.Write(iv);
            writerB.Write(hash);
            writerB.Write(output);

            writerB.Close();
            stream.Close();

            return Convert.ToBase64String(blob);
        }

        #endregion

    }
}