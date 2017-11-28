using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Messenger.API.IO;
using Messenger.IO;
using NUnit.Framework;
using System.Linq;
using System.Threading;

namespace Messenger.API.UnitTesting
{
    [TestFixture]
    public class MessengerReaderUnitTesting
    {
        private static readonly Encoding decoder = new UTF8Encoding(false, false);
        private static readonly Random r = new Random();

        #region Internal

        // Done!
        private static IList<string> RandomSplit(string s)
        {
            List<int> index = Enumerable.Range(0, s.Length - 1).OrderBy(w => r.Next()).Take((int)(s.Length * 0.3)).ToList();
            index.Sort();
            index.RemoveAt(0);
            index.RemoveAt(index.Count-1);

            List<string> data = new List<string> { s.Substring(0, index[0])};

            for (int i = 0; i < index.Count - 1; ++i)
            {
                int a = index[i];
                int b = index[i + 1];
                Assert.AreEqual(b - a >= 1, true);
                data.Add(s.Substring(a, b - a));
            }

            data.Add(s.Substring(index[index.Count - 1]));

            Assert.AreEqual(s, ConvertToString(data));

            return data;
        }

        // Done!
        private static IList<AsyncDataEventArgs> ConvertToAssyncDataCollection(IList<string> command)
        {
            IList<AsyncDataEventArgs> result = new List<AsyncDataEventArgs>();
            for(int i=0; i< command.Count;++i)
            {
                byte[] data = decoder.GetBytes(command[i]);
                Assert.AreEqual(data.Length > 0, true);
                result.Add(new AsyncDataEventArgs(data, i));
            }
            return result;
        }

        // Done!
        private static IList<AsyncDataEventArgs> FragmentAssyncDataCollection(IEnumerable<AsyncDataEventArgs> asyncData)
        {
            IList<AsyncDataEventArgs> result = (from x in asyncData.OrderBy(w => r.Next()) select x).ToList();
            return result;
        }

        // Done!
        private static string ConvertToString(IList<string> data)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < data.Count; ++i)
            {
                sb.Append(data[i]);
            }
            return sb.ToString();
        }

        // Done!
        private static string GenerateString()
        {
            return new string(Enumerable.Range(0, 2000).Select(i => (char)(r.Next() % 1024)).ToArray());
        }

        [Test]
        public void RandomSplitTester()
        {
            for (int i = 0; i < 100; ++i)
            {
                RandomSplitTesterHelper(GenerateString());
            }
        }

        // Done!
        private static void RandomSplitTesterHelper(string a)
        {
            Assert.AreEqual(a, ConvertToString(RandomSplit(a)));
        }

        #endregion

        [Test]
        public void CommandVersionTester()
        {
            CommandVersionTesterHelper("VER 1 MSNP8 MSNP9 MSNP10 MSNP11 MSNP12 MSNP13 MSNP14 MSNP15 CVR0\r\n");

            //for (int i = 0; i < 10; ++i)
            //{
            //    CommandVersionTesterHelper("VER 1 MSNP8 MSNP9 MSNP10 MSNP11 MSNP12 MSNP13 MSNP14 MSNP15 CVR0\r\n");
            //    CommandVersionTesterHelper("VER 2 MSNP8 MSNP14 MSNP15 CVR0\r\n");
            //    CommandVersionTesterHelper("VER 1999 MSNP8 CVR0\r\n");
            //    CommandVersionTesterHelper("VER 346 MSNP8 MSNP9 MSNP13 MSNP14 MSNP15 CVR0\r\n");
            //    CommandVersionTesterHelper("VER 3 MSNP15 CVR0\r\n");
            //    CommandVersionTesterHelper("VER 33 CVR0\r\n");
            //}
        }

        // Done!
        private static void CommandVersionTesterHelper(string s)
        {
            EventHandler<MessageEventArgs> call = (o, e) =>
            {
                Assert.AreEqual(Package.PackageType.Version, e.Package.PackageType);
                Assert.AreEqual(e.Package + "\r\n", s);
            };

            CommandTesterHelper(s, call);

        }

        // Done!
        private static void CommandTesterHelper(string s, EventHandler<MessageEventArgs> call)
        {
            // Vamos enviar isto!
            IList<AsyncDataEventArgs> data = FragmentAssyncDataCollection(ConvertToAssyncDataCollection(RandomSplit(s)));

            AsyncStreamReader ar = CreateAsyncStreamReader();
            MessengerReader mr = new MessengerReader(ar);

            bool done = false;
            mr.MessageReceived += (o, e) =>
            {
                call.Invoke(o, e);
                done = true;
            };

            // Send data!
            for (int i = 0; i < data.Count; ++i)
            {
                ar.Invoke(data[i]);
            }

            int time = Environment.TickCount;

            while(done == false)
            {
                Thread.Sleep(20);
                if(Environment.TickCount - time > 2000)
                {
                    throw new AssertionException("Could not complete operation on time");
                }
            }
        }

        // Done!
        private static AsyncStreamReader CreateAsyncStreamReader()
        {
            MemoryStream ms = new MemoryStream();
            return new AsyncStreamReader(ms);
        }

    }
}
