using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net.NetworkInformation;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Threading;
using EightPuzzle.App;
using Messenger.API;
using Messenger.WebServices;

namespace Messenger.App
{
    public partial class Window1
    {
        private MessengerClient mc;
        public static ObservableCollection<Contact> contacts = new ObservableCollection<Contact>();

        public Window1()
        {
            InitializeComponent();
            ContentHolder.Opacity = 1;
            ContentHolder.Visibility = Visibility.Visible;
            ContactsHolder.Opacity = 0;
            ContactsHolder.Visibility = Visibility.Hidden;
        }

        // Done!
        private void MainWindow_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Interoperability.SendMessage(new WindowInteropHelper(this).Handle, 0xA1, 0x2, 0);
        }

        // Done!
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        // Done!
        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        // Done!
        private void Enter_Click(object sender, RoutedEventArgs e)
        {
            UserImage.IsEnabled = false;
            Enter.IsEnabled = false;
            Loader.Visibility = Visibility.Visible;

            string un = this.UserNameText.Text;
            string up = this.UserPasswordText.Password;

            new Thread(() => this.Connect(un, up)).Start();

            new Thread(() =>
            {
                while (true)
                {
                    Thread.Sleep(200);
                    if (mc.Authenticated)
                    {
                        break;
                    }
                }

                AlterConfigurationDelegate call = this.AlterConfiguration;

                this.Dispatcher.BeginInvoke(DispatcherPriority.Send, call);

                   
            }).Start();
        }

        // Done!
        public void AlterConfiguration()
        {
            if (!Dispatcher.CheckAccess())
            {
                Dispatcher.Invoke(DispatcherPriority.Send, new AlterConfigurationDelegate(this.AlterConfiguration));
                return;
            }

            UserImage.IsEnabled = true;
            Enter.IsEnabled = true;
            Loader.Visibility = Visibility.Hidden;

            Loader.BeginStoryboard(LoaderStoryboard.Storyboard);

            ContentHolder.Opacity = 0;
            ContentHolder.Visibility = Visibility.Hidden;
            ContactsHolder.Opacity = 1;
            ContactsHolder.Visibility = Visibility.Visible;

            //int count = 0;
            
            new Thread(() =>
            {
                AlterConfigurationDelegate call = AlterContacts;
                int time = System.Environment.TickCount;
                while (true)
                {
                    Thread.Sleep(200);
                    if (mc.ContactsSyncronized())
                    {
                        break;
                    }
                }
                this.Dispatcher.BeginInvoke(DispatcherPriority.Send, call);

            }).Start();

        }

        // Done!
        private delegate void AlterConfigurationDelegate();
        
        private void AlterContacts()
        {
            if (!Dispatcher.CheckAccess())
            {
                Dispatcher.Invoke(DispatcherPriority.Send, new AlterConfigurationDelegate(this.AlterConfiguration));
                return;
            }

            foreach (var c in mc.Contacts)
            {
                contacts.Add(c);    
            }
        }

        // Done!
        private void Loader_MouseDown(object sender, MouseButtonEventArgs e)
        {
            UserImage.IsEnabled = true;
            Enter.IsEnabled = true;
            Loader.Visibility = Visibility.Hidden;
        }

        private void Connect(string userName, string userPassword)
        {
            if (NetworkInterface.GetIsNetworkAvailable() == false)
            {
                MessageBox.Show("Offline mode");
                return;
            }

            #region Login Data
            //mc = new MessengerClient("www.ricardo.org@hotmail.com", "ohudhqar");
            mc = new MessengerClient(userName, userPassword);
            #endregion

            // Blocking!
            mc.Authenticate();

            if (mc.Authenticated == false)
            {
                MessageBox.Show("Authenticated Failed");
                return;
            }

            mc.SyncronizeContacts();
            mc.ChangeStatus(ClientStatus.Online);
            mc.ChangePersonalMessage("ÀÁàáÈÉèéÌÍìíÒÓòóÙÚùúÂâÃãÊêÎîÔôÕõÛûÄäËëÏïÖöÜüÇçÑñ");

            //const string user = "www.ricardo.org@hotmail.com";
            //for (int i = 5; i >= 0; --i)
            //{
            //    Console.WriteLine("waiting to cal user: {0} in {1}", user, i*1000);
            //    Thread.Sleep(1000);
            //}

            //mc.InviteUserToChatSession(user);

        }

    }
}
