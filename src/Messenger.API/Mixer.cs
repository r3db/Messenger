using System;
using System.Net;
using Contracts;
using Messenger.API.IO;
using Messenger.API.Package.Request;
using Messenger.API.Package.Response;

namespace Messenger.API
{
    public sealed class Mixer
    {
        private readonly MessengerSocket socket;
        private int attempts = 3;

        #region .ctor

        public Mixer(IPEndPoint endpoint, string account, int sessionID, int timeout)
        {
            endpoint.NotNull();
            account.NotEmpty();

            this.socket = new MessengerSocket(endpoint, timeout);

            this.Account = account;
            this.SessionID = sessionID;
            this.Timeout = timeout;
            this.socket.Reader.MessageReceived += MessageReceived;

        }


        public Mixer(IPEndPoint endpoint, string account, int sessionID)
            : this(endpoint, account, sessionID, 5000)
        { }

        public Mixer(IPEndPoint endpoint)
            : this(endpoint, string.Empty, -1, 5000)
        { }

        #endregion

        #region Properties

        public int Attempts
        {
            get { return this.attempts; }
            set
            {
                if (this.attempts < 0)
                {
                    throw new ArgumentOutOfRangeException("value");
                }
                this.attempts = value; 
            }
        }

        public IPEndPoint Host { get; private set; }
        public string Account { get; private set; }
        public int SessionID { get; private set; }
        public int Timeout { get; private set; }

        #endregion

        private static void MessageReceived(object sender, MessageEventArgs e)
        {
            
        }

        public void AcceptInvitation(string ticket)
        {
            //Nestes casos não queremos apenas uma 'response' com um transactionID igual ao enviado!
            //Queremos isso, mas também queremos um tipo de 'pacote' especial!
            PackageResponse resp1 = this.socket.SendAndBlock<AcceptedChatInvitationResponse>(new AcceptChatInvitationRequest(this.Account, ticket, this.SessionID));
            //Este cast é desnecessário!
            AcceptedChatInvitationResponse resp2 = resp1 as AcceptedChatInvitationResponse;

            if (resp2 == null)
            {
                Console.WriteLine("Error Chat Invitation Failed");
            }
        }

        public void InviteUser(string account)
        {
            this.socket.NotNull();
            account.NotEmpty();

            PackageRequest request = new ChatInvitationRequest(account);

            NotificationServerRedirectionResponse response = this.socket.SendAndBlock(request) as NotificationServerRedirectionResponse;
        }

    }
}