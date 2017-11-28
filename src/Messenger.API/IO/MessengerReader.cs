using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Xml;

using Messenger.API.Package;
using Messenger.API.Package.Request;
using Messenger.API.Package.Response;
using Messenger.API.Parser;
using Messenger.API.Utility;
using Messenger.IO;

//namespace Messenger.API.IO
//{
//    internal sealed class MessengerReader : IAsyncStreamReader
//    {
//        // Done!
//        #region Events

//        public event EventHandler<MessageEventArgs> MessageReceived;

//        #endregion

//        // Done!
//        #region Internal Constants

//        private const string Debug = "DEBUG";
//        private static readonly string inputLabel = new string('<', 32);
//        private static readonly string separatorLabel = new string('-', 32);

//        #endregion

//        // Done!
//        #region Internal Data

//        private readonly AsyncStreamReader reader;
//        private readonly MessageBuffer buffer = new MessageBuffer();
//        private readonly DefragmentationBuffer db = new DefragmentationBuffer();
//        private readonly Encoding decoder = new UTF8Encoding(false, false);

//        #endregion

//        // Done!
//        #region .Ctor

//        // Done!
//        internal MessengerReader(AsyncStreamReader ar)
//        {
//            this.reader = ar;
//            this.reader.DataReceived += this.DataReceived;
//        }

//        // Done!
//        public MessengerReader(Stream stream, int bufferSize, int wait)
//        {
//            this.reader = new AsyncStreamReader(stream, bufferSize, wait);
//            this.reader.DataReceived += this.DataReceived;
//        }

//        // Done!
//        public MessengerReader(Stream stream, int bufferSize)
//            : this(stream, bufferSize, AsyncStreamReader.MinimumWaitTime)
//        {
//        }

//        // Done!
//        public MessengerReader(Stream stream)
//            : this(stream, 4096, AsyncStreamReader.MinimumWaitTime)
//        {
//        }

//        #endregion

//        // Done!
//        #region Interface IAsyncStreamReader

//        // Done!
//        public bool IsListening
//        {
//            get { return this.reader.IsListening; }
//            set { this.reader.IsListening = value; }
//        }

//        // Done!
//        public void StartListening()
//        {
//            this.reader.StartListening();
//        }

//        #endregion

//        // Done!
//        [Conditional(Debug)]
//        private static void ReceiveCommand(string command)
//        {
//            lock (Console.Out)
//            {
//                ConsoleColor backup = Console.ForegroundColor;
//                Console.ForegroundColor = ConsoleColor.Cyan;
//                Console.WriteLine(inputLabel);
//                Console.Write(command);
//                Console.WriteLine(separatorLabel);
//                Console.ForegroundColor = backup;
//            }
//        }

//        // Done!
//        private void Invoke(PackageResponse package)
//        {
//            if (this.MessageReceived != null)
//            {
//                this.MessageReceived.Invoke(this, new MessageEventArgs(package));
//            }
//        }

//        // Done!
//        private void DataReceived(object sender, AsyncDataEventArgs e)
//        {
//            string command = this.decoder.GetString(e.Message());
//            //Console.WriteLine(e.Identifier +  " Command: " + command);
//            this.ParseMessage(e.Identifier, command);
//        }

//        private void ParseMessage(int id, string message)
//        {
//            this.buffer.Store(message, id);
//            if (message.Length <= 4) return;

//            CommandParser parser = new CommandParser(message);

//            string header = parser.ReadBlock(3);
//            switch (header)
//            {
//                #region VER
//                case "VER":
//                    {
//                        MessageContainer result = this.ParseCommand(id, 2, message, parser);
//                        VersionResponse package = new VersionResponse(result.TransactionID, new string[] { result.Parameters[0] });
//                        this.Invoke(package);

//                    }
//                    break;
//                #endregion
//                #region CVR
//                case "CVR":
//                    {
//                        MessageContainer result = this.ParseCommand(id, 6, message, parser);
//                        MachineInfoResponse package = new MachineInfoResponse(
//                            result.TransactionID,
//                            new Version[]
//                        {
//                            new Version(result.Parameters[0]),
//                            new Version(result.Parameters[1])
//                        },
//                            new Version(result.Parameters[2]),
//                            new Uri(result.Parameters[3]),
//                            new Uri(result.Parameters[4])
//                            );
//                        this.Invoke(package);
//                    }
//                    break;
//                #endregion
//                #region XFR
//                case "XFR":
//                    {
//                        MessageContainer result = this.ParseCommand(id, new int[] { 5, 8 }, message, parser);

//                        PackageResponse package = null;

//                        string[] endPoint = result.Parameters[1].Split(':');

//                        switch (result.Parameters[0])
//                        {
//                            case "NS":
//                                {
//                                    package = new DispatchServerRedirectionResponse(
//                                        result.TransactionID,
//                                        new IPEndPoint(IPAddress.Parse(endPoint[0]), int.Parse(endPoint[1]))
//                                        );
//                                    break;
//                                }
//                            case "SB":
//                                {
//                                    package = new NotificationServerRedirectionResponse(
//                                        result.TransactionID,
//                                        new IPEndPoint(IPAddress.Parse(endPoint[0]), int.Parse(endPoint[1])),
//                                        (AuthorizationType)Enum.Parse(typeof(AuthorizationType), result.Parameters[2]),
//                                        result.Parameters[3],
//                                        result.Parameters[5],
//                                        result.Parameters[6] == "1" ? true : false
//                                        );
//                                    break;
//                                }
//                            default:
//                                {
//                                    Console.WriteLine("Error");
//                                    break;
//                                }
//                        }

//                        this.Invoke(package);
//                        break;
//                    }
//                #endregion
//                #region USR
//                case "USR":
//                    {
//                        MessageContainer result = this.ParseCommand(id, 5, message, parser);

//                        PackageResponse package = null;
//                        if (result.Parameters[0] == "SSO")
//                        {
//                            package = new AuthenticationResponse(result.TransactionID, result.Parameters[2], result.Parameters[3]);
//                        }
//                        else if (result.Parameters[0] == "OK")
//                        {
//                            package = new AuthenticationAchievedResponse(result.TransactionID, result.Parameters[2] == "1", result.Parameters[1]);
//                        }
//                        else
//                        {
//                            Console.WriteLine("Error");
//                        }
//                        this.Invoke(package);
//                        break;
//                    }
//                #endregion
//                #region GCF
//                case "GCF":
//                    {
//                        MessageContainer result = this.ParseCommand(id, 2, message, parser, true);
//                        message = result.Message;

//                        int payload = int.Parse(result.Parameters[0]);

//                        if (this.buffer.Defragmenting(id, ref message, ref result, payload, ParseMessage))
//                        {
//                            return;
//                        }

//                        //Agora que temos a mensagem completa!
//                        //Vamos ler o conteúdo da mesma!!!
//                        //int start = message.IndexOf("\r\n") + 2;
//                        //string xml = message.Substring(start, payload);

//                        //Esta é a maneira correcta de ler o conteúdo do payload!
//                        //Porque o payload não vem em carateres!
//                        //Mas sim me bytes
//                        string xml = Encoding.UTF8.GetString(Encoding.UTF8.GetBytes(message.Substring(message.IndexOf("\r\n") + 2)), 0, payload);

//                        XmlDocument xmlDoc = new XmlDocument();
//                        xmlDoc.Load(new StringReader(xml));

//                        DeclarationResponse package = new DeclarationResponse(result.TransactionID, xmlDoc, payload);
//                        this.Invoke(package);

//                        //Vamos verificar se sobrou alguma coisa!
//                        message = message.Substring(message.IndexOf(xml) + xml.Length);
//                        //message = message.Substring(start + payload);
//                        if (!string.IsNullOrEmpty(message))
//                        {
//                            this.ParseMessage(id, message);
//                        }

//                        break;
//                    }
//                #endregion
//                #region SBS
//                case "SBS":
//                    {
//                        MessageContainer result = this.ParseCommand(id, 2, message, parser);

//                        //Só forçado a converter para Lista! Caso contrário, ao que parece, os resultados perdem-se!
//                        int counter = 1;
//                        List<string> protocols = (from x in result.Parameters
//                                                  where counter++ < result.Parameters.Count
//                                                  select x).ToList();

//                        VersionResponse package = new VersionResponse(result.TransactionID, protocols);
//                        this.Invoke(package);
//                        break;
//                    }
//                #endregion
//                #region MSG
//                case "MSG":
//                    {
//                        MessageContainer result = this.ParseCommand(id, 3, message, parser, true);
//                        message = result.Message;

//                        int payload = int.Parse(result.Parameters[2]);

//                        //Temos que ler o conteúdo com atenção ou então vamos ficar sem mensagem!
//                        if (this.buffer.Defragmenting(id, ref message, ref result, payload, ParseMessage))
//                        {
//                            return;
//                        }

//                        //Esta é a maneira correcta de ler o conteúdo do payload!
//                        //Porque o payload não vem em carateres!
//                        //Mas sim me bytes
//                        string content = Encoding.UTF8.GetString(Encoding.UTF8.GetBytes(message.Substring(message.IndexOf("\r\n") + 2)), 0, payload);

//                        Dictionary<string, string> list = new Dictionary<string, string>();
//                        string[] split = content.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
//                        foreach (string item in split)
//                        {
//                            int position = item.IndexOf(':');
//                            if (position == -1) continue;
//                            list.Add(item.Substring(0, position).Trim(), item.Substring(position + 1).Trim());
//                        }

//                        PackageResponse package = null;

//                        if (result.Parameters[1] == "Hotmail")
//                        {
//                            if (list["Content-Type"].IndexOf("text/x-msmsgsprofile") != -1)
//                            {
//                                //Validate profile
//                                package = new ProfileInformationResponse(new Version(list["MIME-Version"]), list["Content-Type"], list, payload);
//                            }
//                            else if (list["Content-Type"].IndexOf("text/x-msmsgsinitialmdatanotification") != -1)
//                            {
//                                //Validate email!
//                                package = new InboxMessageResponse(new Version(list["MIME-Version"]), list["Content-Type"], list, payload);
//                            }
//                            else if (list["Content-Type"].IndexOf("text/x-msmsgsoimnotification") != -1)
//                            {
//                                //OIM?!
//                                package = new OfflineMessageResponse(new Version(list["MIME-Version"]), list["Content-Type"], list, payload);
//                            }
//                            else
//                            {
//                                //Error!
//                                Console.WriteLine("Error in Msg");
//                            }
//                        }
//                        else
//                        {
//                            if (list.ContainsKey("TypingUser"))
//                            {
//                                //Typing....!
//                                package = new UserTypingMessageResponse(new Version(list["MIME-Version"]), list["Content-Type"], list["TypingUser"], list, payload);
//                                list.Remove("TypingUser");
//                            }
//                            else
//                            {
//                                //Enviaram-nos uma mensagem!!!
//                                //Vamos ler!!! Yehhhh!
//                                string msg = content.Substring(content.IndexOf("\r\n\r\n") + "\r\n\r\n".Length);
//                                package = new IncomingUserMessageResponse(new Version(list["MIME-Version"]), list["Content-Type"], list["X-MMS-IM-Format"], msg, list, payload);
//                                list.Remove("X-MMS-IM-Format");
//                            }
//                        }

//                        list.Remove("MIME-Version");
//                        list.Remove("Content-Type");
//                        this.Invoke(package);

//                        //Vamos verificar se sobrou alguma coisa!
//                        //Todo I think this is a bug!
//                        message = message.Substring(message.IndexOf(content) + content.Length);
//                        //message = message.Substring(start + payload);
//                        if (!string.IsNullOrEmpty(message))
//                        {
//                            this.ParseMessage(id, message);
//                        }

//                        break;
//                    }
//                #endregion
//                #region BLP
//                case "BLP":
//                    {
//                        MessageContainer result = this.ParseCommand(id, 2, message, parser);

//                        PrivacySettingsResponse package = new PrivacySettingsResponse(result.TransactionID, result.Parameters[0] == "BL" ? false : true);
//                        this.Invoke(package);
//                        break;
//                    }
//                #endregion
//                #region ADL
//                case "ADL":
//                    {
//                        MessageContainer result = this.ParseCommand(id, 2, message, parser);

//                        if (result.Parameters[0] != "OK")
//                        {
//                            Console.WriteLine("Error");
//                        }

//                        SyncronizeContactsResponse package = new SyncronizeContactsResponse(result.TransactionID);
//                        this.Invoke(package);
//                        break;
//                    }
//                #endregion
//                #region PRP
//                case "PRP":
//                    {
//                        MessageContainer result = this.ParseCommand(id, 3, message, parser);

//                        EditUserPropertiesResponse package = new EditUserPropertiesResponse(result.TransactionID, (UserProperties)Enum.Parse(typeof(UserProperties), result.Parameters[0]), result.Parameters[1]);
//                        this.Invoke(package);
//                        break;
//                    }
//                #endregion
//                #region CHG
//                case "CHG":
//                    {
//                        MessageContainer result = this.ParseCommand(id, new int[] { 2, 3, 4 }, message, parser);

//                        ClientCapabilities cap = (ClientCapabilities)0;
//                        long value;
//                        if (result.Parameters.Count > 1 && long.TryParse(result.Parameters[1], out value))
//                        {
//                            cap = (ClientCapabilities)value;
//                        }

//                        ObjectDescriptor descriptor = null;
//                        if (result.Parameters.Count > 2)
//                        {
//                            descriptor = new ObjectDescriptor(result.Parameters[2]);
//                        }

//                        ChangeStatusResponse package = new ChangeStatusResponse(result.TransactionID, result.Parameters[0].ToClientStatus(), cap, descriptor);
//                        this.Invoke(package);
//                        break;
//                    }
//                #endregion
//                #region UUX
//                case "UUX":
//                    {
//                        MessageContainer result = this.ParseCommand(id, 2, message, parser);

//                        PersonalMessageResponse package = new PersonalMessageResponse(result.TransactionID, result.Parameters[0] == "0" ? true : false);
//                        this.Invoke(package);
//                        break;
//                    }
//                #endregion
//                #region CHL
//                case "CHL":
//                    {
//                        MessageContainer result = this.ParseCommand(id, 2, message, parser);

//                        ChallengeResponse package = new ChallengeResponse(result.TransactionID, result.Parameters[0]);
//                        this.Invoke(package);
//                        break;
//                    }
//                #endregion
//                #region QRY
//                case "QRY":
//                    {
//                        MessageContainer result = this.ParseCommand(id, 1, message, parser);

//                        ChallengeAchievedResponse package = new ChallengeAchievedResponse(result.TransactionID);
//                        this.Invoke(package);
//                        break;
//                    }
//                #endregion
//                #region ILN
//                case "ILN":
//                    {
//                        this.StatusHandler(id, message, parser);
//                        break;
//                    }
//                #endregion
//                #region NLN
//                case "NLN":
//                    {
//                        this.StatusHandler(id, message, parser);
//                        break;
//                    }
//                #endregion
//                #region UBX
//                case "UBX":
//                    {
//                        MessageContainer result = this.ParseCommand(id, 3, message, parser, true);
//                        message = result.Message;

//                        int payload = int.Parse(result.Parameters[2]);

//                        if (this.buffer.Defragmenting(id, ref message, ref result, payload, ParseMessage))
//                        {
//                            return;
//                        }

//                        if (payload == 0)
//                        {
//                            break;
//                            //lock (this.buffer)
//                            //{
//                            //    this.buffer.Add(id, message);
//                            //}
//                        }

//                        //Agora que temos a mensagem completa!
//                        //Vamos ler o conteúdo da mesma!!!
//                        //int start = message.IndexOf("\r\n") + 2;
//                        //string xml = message.Substring(start, payload);

//                        //Esta é a maneira correcta de ler o conteúdo do payload!
//                        //Porque o payload não vem em carateres!
//                        //Mas sim me bytes
//                        string xml = Encoding.UTF8.GetString(Encoding.UTF8.GetBytes(message.Substring(message.IndexOf("\r\n") + 2)), 0, payload);

//                        PersonalMessageNotificationResponse package = new PersonalMessageNotificationResponse(result.Parameters[0], (NetworkId)int.Parse(result.Parameters[1]), xml);
//                        this.Invoke(package);

//                        //Vamos verificar se sobrou alguma coisa!
//                        //message = message.Substring(start + payload);
//                        message = message.Substring(message.IndexOf(xml) + xml.Length);
//                        if (!string.IsNullOrEmpty(message))
//                        {
//                            this.ParseMessage(id, message);
//                        }

//                        break;
//                    }
//                #endregion
//                #region FLN
//                case "FLN":
//                    {
//                        this.StatusHandler(id, message, parser, false);
//                        break;
//                    }
//                #endregion
//                #region RNG
//                case "RNG":
//                    {
//                        MessageContainer result = this.ParseCommand(id, 9, message, parser);

//                        string[] host = result.Parameters[0].Split(':');
//                        ChatInvitationResponse package = new ChatInvitationResponse(
//                            result.TransactionID,
//                            new IPEndPoint(IPAddress.Parse(host[0]), int.Parse(host[1])),
//                            (AuthorizationType)Enum.Parse(typeof(AuthorizationType), result.Parameters[1]),
//                            result.Parameters[2],
//                            result.Parameters[3],
//                            HttpUtility.UrlDecode(result.Parameters[4]),
//                            result.Parameters[6],
//                            result.Parameters[7] == "0" ? false : true);
//                        this.Invoke(package);
//                        break;
//                    }
//                #endregion
//                #region IRO
//                case "IRO":
//                    {
//                        MessageContainer result = this.ParseCommand(id, 6, message, parser);

//                        RoosterResponse package = new RoosterResponse(
//                            result.TransactionID,
//                            int.Parse(result.Parameters[0]),
//                            int.Parse(result.Parameters[1]),
//                            result.Parameters[2],
//                            HttpUtility.UrlDecode(result.Parameters[3]),
//                            (ClientCapabilities)ulong.Parse(result.Parameters[4]));
//                        this.Invoke(package);
//                        break;
//                    }
//                #endregion
//                #region ANS
//                case "ANS":
//                    {
//                        MessageContainer result = this.ParseCommand(id, 2, message, parser);

//                        if (result.Parameters[0] != "OK")
//                        {
//                            break;
//                        }

//                        AcceptedChatInvitationResponse package = new AcceptedChatInvitationResponse(result.TransactionID);
//                        this.Invoke(package);
//                        break;
//                    }
//                #endregion
//                #region BYE
//                case "BYE":
//                    {
//                        MessageContainer result = this.ParseCommand(id, new int[] { 1, 2 }, message, parser);


//                        PrincipalLeftChatResponse package = new PrincipalLeftChatResponse(result.Parameters[0], (result.Parameters.Count > 1 && result.Parameters[1] == "1") ? true : false);
//                        this.Invoke(package);
//                        break;
//                    }
//                #endregion
//                #region Todo
//                case "BPR":
//                    {
//                        break;
//                    }
//                case "GTC":
//                    {
//                        break;
//                    }
//                case "LSG":
//                    {
//                        break;
//                    }
//                case "LST":
//                    {
//                        break;
//                    }
//                //Deprecated since MSNP13
//                case "ADC":
//                    {
//                        break;
//                    }
//                //Deprecated since MSNP11
//                case "ADD":
//                    {
//                        break;
//                    }
//                case "ADG":
//                    {
//                        break;
//                    }
//                case "FQY":
//                    {
//                        break;
//                    }
//                case "OUT":
//                    {
//                        break;
//                    }
//                case "PNG":
//                    {
//                        break;
//                    }
//                case "QNG":
//                    {
//                        break;
//                    }
//                case "SYN":
//                    {
//                        break;
//                    }
//                case "REA":
//                    {
//                        break;
//                    }
//                case "REG":
//                    {
//                        break;
//                    }
//                case "REM":
//                    {
//                        break;
//                    }
//                case "RMG":
//                    {
//                        break;
//                    }
//                case "CAL":
//                    {
//                        break;
//                    }
//                case "JOI":
//                    {
//                        break;
//                    }
//                #endregion
//                #region Default

//                // Todo: Handle error Codes.
//                default:
//                    {
//                        int error;
//                        // A number means an error code!
//                        // Right now we are ignoring error codes.
//                        bool sucess = int.TryParse(header, out error) && header.Length == 5;

//                        if (sucess == false)
//                        {
//                            //Console.WriteLine("ID: " + id);
//                            int length = message.Length;
//                            this.buffer.Store(id, ref message);

//                            // If message size differs from original it means that we had defragmentation!
//                            // Meaning: We have a new message to parse, generated from buffer.
//                            if (length != message.Length)
//                            {
//                                this.ParseMessage(id, message);
//                            }
//                        }

//                        // Even if we have no defragmentation we exit and wait for more incoming messages!

//                        break;
//                    }

//                #endregion
//            }

//        }

//        private void StatusHandler(int id, string message, CommandParser parser, bool hasState)
//        {
//            MessageContainer result = this.ParseCommand(id, new[] { 3, 4, 5, 6, 7 }, message, parser);

//            int offset = hasState ? 0 : -1;

//            string account = string.Empty;
//            if (result.Parameters.Count > (1 + offset))
//            {
//                account = result.Parameters[(1 + offset)];
//            }

//            string nick = string.Empty;
//            if (result.Parameters.Count > (3 + offset))
//            {
//                nick = result.Parameters[(3 + offset)];
//            }

//            NetworkId networkId = NetworkId.None;
//            int intValue;
//            if (result.Parameters.Count > (2 + offset) && int.TryParse(result.Parameters[(2 + offset)], out intValue))
//            {
//                networkId = (NetworkId)intValue;
//            }

//            ClientCapabilities cap = ClientCapabilities.None;
//            long longValue;
//            if (result.Parameters.Count > (4 + offset) && long.TryParse(result.Parameters[(4 + offset)], out longValue))
//            {
//                cap = (ClientCapabilities)longValue;
//            }

//            ObjectDescriptor descriptor = null;
//            if (result.Parameters.Count > (5 + offset))
//            {
//                descriptor = new ObjectDescriptor(result.Parameters[(5 + offset)]);
//            }

//            ChangeStatusNotificationResponse package = new ChangeStatusNotificationResponse(
//                result.TransactionID,
//                account,
//                nick,
//                hasState ? result.Parameters[0].ToClientStatus() : ClientStatus.None,
//                cap,
//                networkId,
//                descriptor);

//            this.Invoke(package);

//        }

//        private void StatusHandler(int id, string message, CommandParser parser)
//        {
//            this.StatusHandler(id, message, parser, true);
//        }

//        //Continue
//        private MessageContainer ParseCommand(int id, IEnumerable<int> expectedTokens, string message, CommandParser parser, bool isPayload)
//        {
//            MessageContainer mc = new MessageContainer { Message = message };

//            List<string> tokens = new List<string>(parser.ReadTokens());

//            bool passed = false;
//            foreach (int item in expectedTokens)
//            {
//                if (tokens.Count == item)
//                {
//                    passed = true;
//                    break;
//                }
//            }
//            if (passed == false)
//            {
//                //Internal Error
//                //Parse According!
//                Console.WriteLine("E:({0}-{1}) R:{2} Message:{3}{4}{5}", expectedTokens.Min(), expectedTokens.Max(), tokens.Count, id, Environment.NewLine, message);
//            }

//            int value;
//            if (!int.TryParse(tokens[0], out value))
//            {
//                value = -1;
//            }

//            mc.TransactionID = value;

//            if (isPayload == false && parser.Length - parser.Position > 2)
//            {
//                string remaining = message.Substring(parser.Position);
//                if (remaining.StartsWith("\r\n"))
//                {
//                    //Neste caso recebemos mais de um commando por mensagem!
//                    remaining = remaining.Substring(2);
//                    //A única dificuldade é que não temos identificador!
//                    //Por agora vamos usar o mesmo!
//                    //Como a mensagem 'actual', (aquela que trazia esta foi "parsed" com sucesso), sabemos que o id se encontra livre!
//                    this.ParseMessage(id, remaining);
//                }
//                message = message.Replace(remaining, string.Empty);
//                mc.Message = message;
//                mc.Remaining = remaining;
//            }

//            //DPO
//            ReceiveCommand(string.Format("{0} : {1}", id, message));

//            mc.Parameters = new List<string>();

//            if (value == -1)
//            {
//                mc.Parameters.AddRange(tokens);
//            }
//            else
//            {
//                int counter = 0;
//                mc.Parameters.AddRange(from x in tokens where counter++ > 0 select x);
//            }

//            return mc;
//        }

//        private MessageContainer ParseCommand(int id, int expectedTokens, string message, CommandParser parser, bool isPayload)
//        {
//            return this.ParseCommand(id, new int[] { expectedTokens }, message, parser, isPayload);
//        }

//        private MessageContainer ParseCommand(int id, IEnumerable<int> expectedTokens, string message, CommandParser parser)
//        {
//            return this.ParseCommand(id, expectedTokens, message, parser, false);
//        }

//        private MessageContainer ParseCommand(int id, int expectedTokens, string message, CommandParser parser)
//        {
//            return this.ParseCommand(id, new int[] { expectedTokens }, message, parser);
//        }

//        // Done!
//        #region IDisposable

//        // Done!
//        public void Dispose()
//        {
//            if (this.reader == null) return;

//            this.reader.Dispose();
//        }

//        #endregion
//    }
//}

namespace Messenger.API.IO
{
    internal sealed class MessengerReader : IAsyncStreamReader
    {
        // Done!
        #region Events

        public event EventHandler<MessageEventArgs> MessageReceived;

        #endregion

        // Done!
        #region Internal Constants

        private const string Debug = "DEBUG";
        private static readonly string inputLabel = new string('<', 32);
        private static readonly string separatorLabel = new string('-', 32);
        private const string NewLine = "\r\n";

        #endregion

        // Done!
        #region Internal Data

        private readonly AsyncStreamReader reader;
        private readonly MessageBuffer buffer = new MessageBuffer();
        private readonly DefragmentationBuffer db = new DefragmentationBuffer();
        private readonly Encoding decoder = new UTF8Encoding(false, false);

        #endregion

        // Done!
        #region .Ctor

        // Done!
        internal MessengerReader(AsyncStreamReader ar)
        {
            this.reader = ar;
            this.reader.DataReceived += this.DataReceived;
        }

        // Done!
        public MessengerReader(Stream stream, int bufferSize, int wait)
        {
            this.reader = new AsyncStreamReader(stream, bufferSize, wait);
            this.reader.DataReceived += this.DataReceived;
        }

        // Done!
        public MessengerReader(Stream stream, int bufferSize)
            : this(stream, bufferSize, AsyncStreamReader.MinimumWaitTime)
        {
        }

        // Done!
        public MessengerReader(Stream stream)
            : this(stream, 4096, AsyncStreamReader.MinimumWaitTime)
        {
        }

        #endregion

        // Done!
        #region Interface IAsyncStreamReader

        // Done!
        public bool IsListening
        {
            get { return this.reader.IsListening; }
            set { this.reader.IsListening = value; }
        }

        // Done!
        public void StartListening()
        {
            this.reader.StartListening();
        }

        #endregion

        // Done!
        [Conditional(Debug)]
        private static void ReceiveCommand(string command)
        {
            lock (Console.Out)
            {
                ConsoleColor backup = Console.ForegroundColor;
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine(inputLabel);
                Console.Write(command);
                Console.WriteLine(separatorLabel);
                Console.ForegroundColor = backup;
            }
        }

        // Done!
        private void Invoke(PackageResponse package)
        {
            if (this.MessageReceived != null)
            {
                this.MessageReceived.Invoke(this, new MessageEventArgs(package));
            }
        }

        // Done!
        private void DataReceived(object sender, AsyncDataEventArgs e)
        {
            string command = this.decoder.GetString(e.Message());
            this.ParseMessage(e.Identifier, command);
        }

        private void ParseMessage(int id, string message)
        {
            db.Add(id, message);

            if (db.ContentSize < 5)
            {
                return;
            }

            //if (db.HasContent())
            //{
            //    message = db.GetContent();
            //}

            this.buffer.Store(message, id);
            if (message.Length <= 4) return;

            CommandParser parser = new CommandParser(message);

            string header = parser.ReadBlock(3);
            switch (header)
            {
                #region VER
                case "VER":
                {
                    if(db.MatchPatternAtEnd(NewLine) == false) { return; }

                    MessageContainer result = this.ParseCommand(id, 2, message, parser);
                    VersionResponse package = new VersionResponse(result.TransactionID, new string[] { result.Parameters[0] });
                    this.Invoke(package);

                }
                break;
                #endregion
                #region CVR
                case "CVR":
                {
                    //if (db.MatchPatternAtEnd(NewLine) == false) { return; }

                    MessageContainer result = this.ParseCommand(id, 6, message, parser);
                    MachineInfoResponse package = new MachineInfoResponse(
                        result.TransactionID,
                        new Version[]
                    {
                        new Version(result.Parameters[0]),
                        new Version(result.Parameters[1])
                    },
                        new Version(result.Parameters[2]),
                        new Uri(result.Parameters[3]),
                        new Uri(result.Parameters[4])
                        );
                    this.Invoke(package);
                }
                break;
                #endregion
                #region XFR
                case "XFR":
                {
                    //if (db.MatchPatternAtEnd(NewLine) == false) { return; }

                    MessageContainer result = this.ParseCommand(id, new int[] { 5, 8 }, message, parser);

                    PackageResponse package = null;

                    string[] endPoint = result.Parameters[1].Split(':');

                    switch (result.Parameters[0])
                    {
                        case "NS":
                            {
                                package = new DispatchServerRedirectionResponse(
                                    result.TransactionID,
                                    new IPEndPoint(IPAddress.Parse(endPoint[0]), int.Parse(endPoint[1]))
                                    );
                                break;
                            }
                        case "SB":
                            {
                                package = new NotificationServerRedirectionResponse(
                                    result.TransactionID,
                                    new IPEndPoint(IPAddress.Parse(endPoint[0]), int.Parse(endPoint[1])),
                                    (AuthorizationType)Enum.Parse(typeof(AuthorizationType), result.Parameters[2]),
                                    result.Parameters[3],
                                    result.Parameters[5],
                                    result.Parameters[6] == "1" ? true : false
                                    );
                                break;
                            }
                        default:
                            {
                                Console.WriteLine("Error");
                                break;
                            }
                    }

                    this.Invoke(package);
                    break;
                }
                #endregion
                #region USR
                case "USR":
                {
                    //if (db.MatchPatternAtEnd(NewLine) == false) { return; }

                    MessageContainer result = this.ParseCommand(id, 5, message, parser);

                    PackageResponse package = null;
                    if (result.Parameters[0] == "SSO")
                    {
                        package = new AuthenticationResponse(result.TransactionID, result.Parameters[2], result.Parameters[3]);
                    }
                    else if (result.Parameters[0] == "OK")
                    {
                        package = new AuthenticationAchievedResponse(result.TransactionID, result.Parameters[2] == "1", result.Parameters[1]);
                    }
                    else
                    {
                        Console.WriteLine("Error");
                    }
                    this.Invoke(package);
                    break;
                }
                #endregion
                #region GCF
                case "GCF":
                {
                    //if (db.MatchPatternAtEnd(NewLine) == false)
                    //{
                    //    return;
                    //}

                    MessageContainer result = this.ParseCommand(id, 2, message, parser, true);
                    message = result.Message;

                    int payload = int.Parse(result.Parameters[0]);

                    if (this.buffer.Defragmenting(id, ref message, ref result, payload, ParseMessage))
                    {
                        return;
                    }

                    //Agora que temos a mensagem completa!
                    //Vamos ler o conteúdo da mesma!!!
                    //int start = message.IndexOf("\r\n") + 2;
                    //string xml = message.Substring(start, payload);

                    //Esta é a maneira correcta de ler o conteúdo do payload!
                    //Porque o payload não vem em carateres!
                    //Mas sim me bytes
                    string xml = Encoding.UTF8.GetString(Encoding.UTF8.GetBytes(message.Substring(message.IndexOf("\r\n") + 2)), 0, payload);

                    XmlDocument xmlDoc = new XmlDocument();
                    xmlDoc.Load(new StringReader(xml));

                    DeclarationResponse package = new DeclarationResponse(result.TransactionID, xmlDoc, payload);
                    this.Invoke(package);

                    //Vamos verificar se sobrou alguma coisa!
                    message = message.Substring(message.IndexOf(xml) + xml.Length);
                    //message = message.Substring(start + payload);
                    if (!string.IsNullOrEmpty(message))
                    {
                        this.ParseMessage(id, message);
                    }

                    break;
                }
                #endregion
                #region SBS
                case "SBS":
                    {
                        MessageContainer result = this.ParseCommand(id, 2, message, parser);

                        //Só forçado a converter para Lista! Caso contrário, ao que parece, os resultados perdem-se!
                        int counter = 1;
                        List<string> protocols = (from x in result.Parameters
                                                  where counter++ < result.Parameters.Count
                                                  select x).ToList();

                        VersionResponse package = new VersionResponse(result.TransactionID, protocols);
                        this.Invoke(package);
                        break;
                    }
                #endregion
                #region MSG
                case "MSG":
                    {
                        MessageContainer result = this.ParseCommand(id, 3, message, parser, true);
                        message = result.Message;

                        int payload = int.Parse(result.Parameters[2]);

                        //Temos que ler o conteúdo com atenção ou então vamos ficar sem mensagem!
                        if (this.buffer.Defragmenting(id, ref message, ref result, payload, ParseMessage))
                        {
                            return;
                        }

                        //Esta é a maneira correcta de ler o conteúdo do payload!
                        //Porque o payload não vem em carateres!
                        //Mas sim me bytes
                        string content = Encoding.UTF8.GetString(Encoding.UTF8.GetBytes(message.Substring(message.IndexOf("\r\n") + 2)), 0, payload);

                        Dictionary<string, string> list = new Dictionary<string, string>();
                        string[] split = content.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries);
                        foreach (string item in split)
                        {
                            int position = item.IndexOf(':');
                            if (position == -1) continue;
                            list.Add(item.Substring(0, position).Trim(), item.Substring(position + 1).Trim());
                        }

                        PackageResponse package = null;

                        if (result.Parameters[1] == "Hotmail")
                        {
                            if (list["Content-Type"].IndexOf("text/x-msmsgsprofile") != -1)
                            {
                                //Validate profile
                                package = new ProfileInformationResponse(new Version(list["MIME-Version"]), list["Content-Type"], list, payload);
                            }
                            else if (list["Content-Type"].IndexOf("text/x-msmsgsinitialmdatanotification") != -1)
                            {
                                //Validate email!
                                package = new InboxMessageResponse(new Version(list["MIME-Version"]), list["Content-Type"], list, payload);
                            }
                            else if (list["Content-Type"].IndexOf("text/x-msmsgsoimnotification") != -1)
                            {
                                //OIM?!
                                package = new OfflineMessageResponse(new Version(list["MIME-Version"]), list["Content-Type"], list, payload);
                            }
                            else
                            {
                                //Error!
                                Console.WriteLine("Error in Msg");
                            }
                        }
                        else
                        {
                            if (list.ContainsKey("TypingUser"))
                            {
                                //Typing....!
                                package = new UserTypingMessageResponse(new Version(list["MIME-Version"]), list["Content-Type"], list["TypingUser"], list, payload);
                                list.Remove("TypingUser");
                            }
                            else
                            {
                                //Enviaram-nos uma mensagem!!!
                                //Vamos ler!!! Yehhhh!
                                string msg = content.Substring(content.IndexOf("\r\n\r\n") + "\r\n\r\n".Length);
                                package = new IncomingUserMessageResponse(new Version(list["MIME-Version"]), list["Content-Type"], list["X-MMS-IM-Format"], msg, list, payload);
                                list.Remove("X-MMS-IM-Format");
                            }
                        }

                        list.Remove("MIME-Version");
                        list.Remove("Content-Type");
                        this.Invoke(package);

                        //Vamos verificar se sobrou alguma coisa!
                        //Todo I think this is a bug!
                        message = message.Substring(message.IndexOf(content) + content.Length);
                        //message = message.Substring(start + payload);
                        if (!string.IsNullOrEmpty(message))
                        {
                            this.ParseMessage(id, message);
                        }

                        break;
                    }
                #endregion
                #region BLP
                case "BLP":
                    {
                        MessageContainer result = this.ParseCommand(id, 2, message, parser);

                        PrivacySettingsResponse package = new PrivacySettingsResponse(result.TransactionID, result.Parameters[0] == "BL" ? false : true);
                        this.Invoke(package);
                        break;
                    }
                #endregion
                #region ADL
                case "ADL":
                    {
                        MessageContainer result = this.ParseCommand(id, 2, message, parser);

                        if (result.Parameters[0] != "OK")
                        {
                            Console.WriteLine("Error");
                        }

                        SyncronizeContactsResponse package = new SyncronizeContactsResponse(result.TransactionID);
                        this.Invoke(package);
                        break;
                    }
                #endregion
                #region PRP
                case "PRP":
                    {
                        MessageContainer result = this.ParseCommand(id, 3, message, parser);

                        EditUserPropertiesResponse package = new EditUserPropertiesResponse(result.TransactionID, (UserProperties)Enum.Parse(typeof(UserProperties), result.Parameters[0]), result.Parameters[1]);
                        this.Invoke(package);
                        break;
                    }
                #endregion
                #region CHG
                case "CHG":
                    {
                        MessageContainer result = this.ParseCommand(id, new int[] { 2, 3, 4 }, message, parser);

                        ClientCapabilities cap = (ClientCapabilities)0;
                        long value;
                        if (result.Parameters.Count > 1 && long.TryParse(result.Parameters[1], out value))
                        {
                            cap = (ClientCapabilities)value;
                        }

                        ObjectDescriptor descriptor = null;
                        if (result.Parameters.Count > 2)
                        {
                            descriptor = new ObjectDescriptor(result.Parameters[2]);
                        }

                        ChangeStatusResponse package = new ChangeStatusResponse(result.TransactionID, result.Parameters[0].ToClientStatus(), cap, descriptor);
                        this.Invoke(package);
                        break;
                    }
                #endregion
                #region UUX
                case "UUX":
                    {
                        MessageContainer result = this.ParseCommand(id, 2, message, parser);

                        PersonalMessageResponse package = new PersonalMessageResponse(result.TransactionID, result.Parameters[0] == "0" ? true : false);
                        this.Invoke(package);
                        break;
                    }
                #endregion
                #region CHL
                case "CHL":
                    {
                        MessageContainer result = this.ParseCommand(id, 2, message, parser);

                        ChallengeResponse package = new ChallengeResponse(result.TransactionID, result.Parameters[0]);
                        this.Invoke(package);
                        break;
                    }
                #endregion
                #region QRY
                case "QRY":
                    {
                        MessageContainer result = this.ParseCommand(id, 1, message, parser);

                        ChallengeAchievedResponse package = new ChallengeAchievedResponse(result.TransactionID);
                        this.Invoke(package);
                        break;
                    }
                #endregion
                #region ILN
                case "ILN":
                    {
                        this.StatusHandler(id, message, parser);
                        break;
                    }
                #endregion
                #region NLN
                case "NLN":
                    {
                        this.StatusHandler(id, message, parser);
                        break;
                    }
                #endregion
                #region UBX
                case "UBX":
                    {
                        MessageContainer result = this.ParseCommand(id, 3, message, parser, true);
                        message = result.Message;

                        int payload = int.Parse(result.Parameters[2]);

                        if (this.buffer.Defragmenting(id, ref message, ref result, payload, ParseMessage))
                        {
                            return;
                        }

                        if (payload == 0)
                        {
                            break;
                            //lock (this.buffer)
                            //{
                            //    this.buffer.Add(id, message);
                            //}
                        }

                        //Agora que temos a mensagem completa!
                        //Vamos ler o conteúdo da mesma!!!
                        //int start = message.IndexOf("\r\n") + 2;
                        //string xml = message.Substring(start, payload);

                        //Esta é a maneira correcta de ler o conteúdo do payload!
                        //Porque o payload não vem em carateres!
                        //Mas sim me bytes
                        string xml = Encoding.UTF8.GetString(Encoding.UTF8.GetBytes(message.Substring(message.IndexOf("\r\n") + 2)), 0, payload);

                        PersonalMessageNotificationResponse package = new PersonalMessageNotificationResponse(result.Parameters[0], (NetworkId)int.Parse(result.Parameters[1]), xml);
                        this.Invoke(package);

                        //Vamos verificar se sobrou alguma coisa!
                        //message = message.Substring(start + payload);
                        message = message.Substring(message.IndexOf(xml) + xml.Length);
                        if (!string.IsNullOrEmpty(message))
                        {
                            this.ParseMessage(id, message);
                        }

                        break;
                    }
                #endregion
                #region FLN
                case "FLN":
                    {
                        this.StatusHandler(id, message, parser, false);
                        break;
                    }
                #endregion
                #region RNG
                case "RNG":
                    {
                        MessageContainer result = this.ParseCommand(id, 9, message, parser);

                        string[] host = result.Parameters[0].Split(':');
                        ChatInvitationResponse package = new ChatInvitationResponse(
                            result.TransactionID,
                            new IPEndPoint(IPAddress.Parse(host[0]), int.Parse(host[1])),
                            (AuthorizationType)Enum.Parse(typeof(AuthorizationType), result.Parameters[1]),
                            result.Parameters[2],
                            result.Parameters[3],
                            HttpUtility.UrlDecode(result.Parameters[4]),
                            result.Parameters[6],
                            result.Parameters[7] == "0" ? false : true);
                        this.Invoke(package);
                        break;
                    }
                #endregion
                #region IRO
                case "IRO":
                    {
                        MessageContainer result = this.ParseCommand(id, 6, message, parser);

                        RoosterResponse package = new RoosterResponse(
                            result.TransactionID,
                            int.Parse(result.Parameters[0]),
                            int.Parse(result.Parameters[1]),
                            result.Parameters[2],
                            HttpUtility.UrlDecode(result.Parameters[3]),
                            (ClientCapabilities)ulong.Parse(result.Parameters[4]));
                        this.Invoke(package);
                        break;
                    }
                #endregion
                #region ANS
                case "ANS":
                    {
                        MessageContainer result = this.ParseCommand(id, 2, message, parser);

                        if (result.Parameters[0] != "OK")
                        {
                            break;
                        }

                        AcceptedChatInvitationResponse package = new AcceptedChatInvitationResponse(result.TransactionID);
                        this.Invoke(package);
                        break;
                    }
                #endregion
                #region BYE
                case "BYE":
                    {
                        MessageContainer result = this.ParseCommand(id, new int[] { 1, 2 }, message, parser);


                        PrincipalLeftChatResponse package = new PrincipalLeftChatResponse(result.Parameters[0], (result.Parameters.Count > 1 && result.Parameters[1] == "1") ? true : false);
                        this.Invoke(package);
                        break;
                    }
                #endregion
                #region Todo
                case "BPR":
                    {
                        break;
                    }
                case "GTC":
                    {
                        break;
                    }
                case "LSG":
                    {
                        break;
                    }
                case "LST":
                    {
                        break;
                    }
                //Deprecated since MSNP13
                case "ADC":
                    {
                        break;
                    }
                //Deprecated since MSNP11
                case "ADD":
                    {
                        break;
                    }
                case "ADG":
                    {
                        break;
                    }
                case "FQY":
                    {
                        break;
                    }
                case "OUT":
                    {
                        break;
                    }
                case "PNG":
                    {
                        break;
                    }
                case "QNG":
                    {
                        break;
                    }
                case "SYN":
                    {
                        break;
                    }
                case "REA":
                    {
                        break;
                    }
                case "REG":
                    {
                        break;
                    }
                case "REM":
                    {
                        break;
                    }
                case "RMG":
                    {
                        break;
                    }
                case "CAL":
                    {
                        break;
                    }
                case "JOI":
                    {
                        break;
                    }
                #endregion
                #region Default

                // Todo: Handle error Codes.
                default:
                    {
                        int error;
                        // A number means an error code!
                        // Right now we are ignoring error codes.
                        bool sucess = int.TryParse(header, out error) && header.Length == 5;

                        if (sucess == false)
                        {
                            //Console.WriteLine("ID: " + id);
                            int length = message.Length;
                            this.buffer.Store(id, ref message);

                            // If message size differs from original it means that we had defragmentation!
                            // Meaning: We have a new message to parse, generated from buffer.
                            if (length != message.Length)
                            {
                                this.ParseMessage(id, message);
                            }
                        }

                        // Even if we have no defragmentation we exit and wait for more incoming messages!

                        break;
                    }

                #endregion
            }

        }

        private void StatusHandler(int id, string message, CommandParser parser, bool hasState)
        {
            MessageContainer result = this.ParseCommand(id, new[] { 3, 4, 5, 6, 7 }, message, parser);

            int offset = hasState ? 0 : -1;

            string account = string.Empty;
            if (result.Parameters.Count > (1 + offset))
            {
                account = result.Parameters[(1 + offset)];
            }

            string nick = string.Empty;
            if (result.Parameters.Count > (3 + offset))
            {
                nick = result.Parameters[(3 + offset)];
            }

            NetworkId networkId = NetworkId.None;
            int intValue;
            if (result.Parameters.Count > (2 + offset) && int.TryParse(result.Parameters[(2 + offset)], out intValue))
            {
                networkId = (NetworkId)intValue;
            }

            ClientCapabilities cap = ClientCapabilities.None;
            long longValue;
            if (result.Parameters.Count > (4 + offset) && long.TryParse(result.Parameters[(4 + offset)], out longValue))
            {
                cap = (ClientCapabilities)longValue;
            }

            ObjectDescriptor descriptor = null;
            if (result.Parameters.Count > (5 + offset))
            {
                descriptor = new ObjectDescriptor(result.Parameters[(5 + offset)]);
            }

            ChangeStatusNotificationResponse package = new ChangeStatusNotificationResponse(
                result.TransactionID,
                account,
                nick,
                hasState ? result.Parameters[0].ToClientStatus() : ClientStatus.None,
                cap,
                networkId,
                descriptor);

            this.Invoke(package);

        }

        private void StatusHandler(int id, string message, CommandParser parser)
        {
            this.StatusHandler(id, message, parser, true);
        }

        //Continue
        private MessageContainer ParseCommand(int id, IEnumerable<int> expectedTokens, string message, CommandParser parser, bool isPayload)
        {
            MessageContainer mc = new MessageContainer { Message = message };

            List<string> tokens = new List<string>(parser.ReadTokens());

            bool passed = false;
            foreach (int item in expectedTokens)
            {
                if (tokens.Count == item)
                {
                    passed = true;
                    break;
                }
            }
            if (passed == false)
            {
                //Internal Error
                //Parse According!
                Console.WriteLine("E:({0}-{1}) R:{2} Message:{3}{4}{5}", expectedTokens.Min(), expectedTokens.Max(), tokens.Count, id, Environment.NewLine, message);
            }

            int value;
            if (!int.TryParse(tokens[0], out value))
            {
                value = -1;
            }

            mc.TransactionID = value;

            if (isPayload == false && parser.Length - parser.Position > 2)
            {
                string remaining = message.Substring(parser.Position);
                if (remaining.StartsWith("\r\n"))
                {
                    //Neste caso recebemos mais de um commando por mensagem!
                    remaining = remaining.Substring(2);
                    //A única dificuldade é que não temos identificador!
                    //Por agora vamos usar o mesmo!
                    //Como a mensagem 'actual', (aquela que trazia esta foi "parsed" com sucesso), sabemos que o id se encontra livre!
                    this.ParseMessage(id, remaining);
                }
                message = message.Replace(remaining, string.Empty);
                mc.Message = message;
                mc.Remaining = remaining;
            }

            //DPO
            ReceiveCommand(string.Format("{0} : {1}", id, message));

            mc.Parameters = new List<string>();

            if (value == -1)
            {
                mc.Parameters.AddRange(tokens);
            }
            else
            {
                int counter = 0;
                mc.Parameters.AddRange(from x in tokens where counter++ > 0 select x);
            }

            return mc;
        }

        private MessageContainer ParseCommand(int id, int expectedTokens, string message, CommandParser parser, bool isPayload)
        {
            return this.ParseCommand(id, new int[] { expectedTokens }, message, parser, isPayload);
        }

        private MessageContainer ParseCommand(int id, IEnumerable<int> expectedTokens, string message, CommandParser parser)
        {
            return this.ParseCommand(id, expectedTokens, message, parser, false);
        }

        private MessageContainer ParseCommand(int id, int expectedTokens, string message, CommandParser parser)
        {
            return this.ParseCommand(id, new int[] { expectedTokens }, message, parser);
        }

        // Done!
        #region IDisposable

        // Done!
        public void Dispose()
        {
            if (this.reader == null) return;

            this.reader.Dispose();
        }

        #endregion
    }
}