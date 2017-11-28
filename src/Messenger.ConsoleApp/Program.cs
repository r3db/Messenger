using System.Net.NetworkInformation;
using Messenger.API;
using System.Diagnostics;

namespace System.Net.Protocols.MSN
{
    public class Program
    {
        public static void Main(string[] arguments)
        {

            if(NetworkInterface.GetIsNetworkAvailable() == false)
            {
                Console.WriteLine("Offline mode");
                Console.ReadLine();
                return;
            }

            if(Debugger.IsAttached)
            {
                ConfigureConsole();
            }

            #region Login Data
            //MessengerClient mc = new MessengerClient("www.ricardo.org@hotmail.com", "ohudhqar");
            MessengerClient mc = new MessengerClient("eusouoironman@hotmail.com", "ohudhqar");
            #endregion

            // Blocking!
            mc.Authenticate();

            if (mc.Authenticated == false)
            {
                Console.WriteLine("Authenticated Failed");
                Console.ReadLine();
                return;
            }

            mc.SyncronizeContacts();
            mc.ChangeStatus(ClientStatus.Online);
            mc.ChangePersonalMessage("ÀÁàáÈÉèéÌÍìíÒÓòóÙÚùúÂâÃãÊêÎîÔôÕõÛûÄäËëÏïÖöÜüÇçÑñ");
           
            Console.WriteLine("Hello");

            //const string user = "www.ricardo.org@hotmail.com";
            //for (int i = 5; i >= 0; --i)
            //{
            //    Console.WriteLine("waiting to cal user: {0} in {1}", user, i*1000);
            //    Thread.Sleep(1000);
            //}

            //mc.InviteUserToChatSession(user);

            Console.ReadLine();

        }

        // Done!
        private static void ConfigureConsole()
        {
            Console.BackgroundColor = ConsoleColor.Black;
            Console.Title = "Messenger.ConsoleApp";

            Console.WindowHeight = 60;
            Console.WindowWidth = 180;
            Console.BufferHeight = 8000;
            Console.BufferWidth = 800;
        }

    }
}                      