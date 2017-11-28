using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Protocols.Messenger;
using System.Threading;
using Contracts;
using Messenger.API.IO;
using Messenger.API.Package;
using Messenger.API.Package.Request;
using Messenger.API.Package.Response;
using Messenger.WebServices;
using Contact = Messenger.WebServices.Contact;
using PassportCredentials = Messenger.WebServices.PassportCredentials;

namespace Messenger.API
{
    public sealed class MessengerClient
    {
        private MessengerSocket socket;
        private int attempts = 3;

        public string Account { get; private set; }
        public string Password { get; private set; }
        public IDictionary<string, PassportCredentials> Credentials { get; set; }
        public IList<Membership> Membership { get; set; }
        public IList<Contact> Contacts { get; set; }
        public bool Authenticated { get; private set; }

        public MessengerClient(string account, string password)
        {
            this.Account = account;
            this.Password = password;
        }

        

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

        #endregion

        private IPEndPoint ConnectToDispathServer()
        {
            MessengerSocket socketP = new MessengerSocket();
            //socket.AutoFlush = false;

            #region Request

            //Request this Data!
            PackageRequest[] request = new PackageRequest[]
                                           { 
                                               new VersionRequest(), 
                                               new MachineInfoRequest(this.Account),
                                               new AuthenticationRequest(this.Account)
                                           };

            //Get responses from the server!
            Dictionary<int, PackageTransaction> responses = socketP.SendRangeAndBlock(request);

            #endregion

            #region Parse Responses if Any

            //Agora que temos as respostas vamos verificar se estão correctas!
            //Seja em quantidade, seja em 'qualidade'...

            if (responses.Count != 3)
            {
                throw new MessengerException("Unable to receive one or more responses from the server");
            }

            //Responses!
            VersionResponse version = responses[request[0].TransactionID].Response as VersionResponse;
            MachineInfoResponse machine = responses[request[1].TransactionID].Response as MachineInfoResponse;
            DispatchServerRedirectionResponse redirection = responses[request[2].TransactionID].Response as DispatchServerRedirectionResponse;

            //Verificar se recebemos as respostas certas!
            version.NotNull<VersionResponse, MessengerProtocolException>("Unexpected response");
            machine.NotNull<MachineInfoResponse, MessengerProtocolException>("Unexpected response");
            redirection.NotNull<DispatchServerRedirectionResponse, MessengerProtocolException>("Unexpected response");

            //Verificar se é uma versão suportada do protocolo!
            version.Protocols.MustContainElement<string, MessengerProtocolException>("MSNP15", "Unsupported  Protocol");

            //Verificar se o nosso cliente ainda é aceitável!
            if ((request[1] as MachineInfoRequest).ClientVersion < machine.MinimumSafeVersion)
            {
                //Ignore! We are not a client! We are just the libraries!
                //throw new Exception("Unsupported  client");
            }

            #endregion

            return redirection.Host;
        }

        #region 'Event' Handlers

        private void EventParser(object sender, MessageEventArgs e)
        {
            ChallengeResponse response = e.Package as ChallengeResponse;

            if (response != null && this.socket != null)
            {
                this.ChallengeResponder(response);
            }

            ChatInvitationResponse response2 = e.Package as ChatInvitationResponse;

            if (response2 != null && this.socket != null)
            {
                this.StartSwitchboardSession(response2);
            }

        }

        private void ChallengeResponder(ChallengeResponse response)
        {
            new Thread(() =>
                           {
                               PackageResponse resp1 = this.socket.SendAndBlock(new ChallengeRequest(response.Challenge));
                               ChallengeAchievedResponse resp2 = resp1 as ChallengeAchievedResponse;

                               if (resp2 == null)
                               {
                                   Console.WriteLine("Error Challenge Failed");
                               }

                           }).Start();
        }

        private void StartSwitchboardSession(ChatInvitationResponse response)
        {
            new Thread(() =>
                           {
                               Mixer swb = new Mixer(response.Host, this.Account, response.SessionID);
                               swb.AcceptInvitation(response.Ticket);

                           }).Start();
        }
        
        #endregion

        #region Authenticate

        private void Authenticate(IPEndPoint host, int attemptsP)
        {
            this.socket = new MessengerSocket(host) {AutoFlush = false};

            //Agora que estamos ligados ao Notification Server vamos começar o processo de autenticação!!!

            #region Request

            //Request this Data!
            PackageRequest[] request = new PackageRequest[]
                                           { 
                                               new VersionRequest(), 
                                               new MachineInfoRequest(this.Account),
                                               new AuthenticationRequest(this.Account)
                                           };

            //Get responses from the server!
            Dictionary<int, PackageTransaction> responses = this.socket.SendRangeAndBlock(request);

            #endregion

            #region Parse Responses if Any

            //Agora que temos as respostas vamos verificar se estão correctas!
            //Seja em quantidade, seja em 'qualidade'...

            if (responses.Count != 3)
            {
                throw new MessengerException("Unable to receive one or more responses from the server");
            }

            //Responses!
            VersionResponse version = responses[request[0].TransactionID].Response as VersionResponse;
            MachineInfoResponse machine = responses[request[1].TransactionID].Response as MachineInfoResponse;
            AuthenticationResponse authentication = responses[request[2].TransactionID].Response as AuthenticationResponse;

            //Verificar se recebemos as respostas certas!
            version.NotNull<VersionResponse, MessengerProtocolException>("Unexpected response");
            machine.NotNull<MachineInfoResponse, MessengerProtocolException>("Unexpected response");

            if (authentication == null)
            { 
                //Vamos ver se fomos redirecionados para outro servidor!
                DispatchServerRedirectionResponse redirection = responses[request[2].TransactionID].Response as DispatchServerRedirectionResponse;
                redirection.NotNull<DispatchServerRedirectionResponse, MessengerProtocolException>("Unexpected response");
                //Caso tenhamos sido redireccionados novamente!
                //Vamos tentar autenticação ai!
                if (attemptsP >= 0)
                {
                    this.Authenticate(redirection.Host, attemptsP - 1);
                }
                return;
            }

            //Verificar se é uma versão suportada do protocolo!
            version.Protocols.MustContainElement<string, MessengerProtocolException>("MSNP15", "Unsupported  Protocol");

            //Verificar se o nosso cliente ainda é aceitável!
            if ((request[1] as MachineInfoRequest).ClientVersion < machine.MinimumSafeVersion)
            {
                //Ignore! We are not a client! We are just the libraries!
                //throw new Exception("Unsupported  client");
            }

            #endregion

            //Como chegamos até aqui
            //Temos confiança que podemos prosseguir com a autenticação!

            #region Request Credentials

            //Vamos obter as nossas credenciais!
            //E neste caso vamos obter credencias para todos os serviços!
            PassportRequest pr = new PassportRequest(this.Account, this.Password, authentication.Policy);

            this.Credentials = pr.RequestCredentials();

            #endregion

            //Agora que temos as credenciais!
            //Vamos POR FIM tentar o 'passo final' de autenticação

            #region Validade Credentials

            const string authDomain = "messengerclear.live.com";
            if (!this.Credentials.ContainsKey(authDomain))
            {
                throw new MessengerAuthenticationException("Could not retrieve Credentials");
            }

            string securityToken = this.Credentials[authDomain].SecurityToken;
            string binarySecret = this.Credentials[authDomain].BinarySecret;

            securityToken.NotEmpty<MessengerAuthenticationException>("Could not retrieve Credentials");
            binarySecret.NotEmpty<MessengerAuthenticationException>("Could not retrieve Credentials");

            string finalTicket = MessengerSocket.CalculateMBIResponse(binarySecret, authentication.Nonce);

            #endregion

            #region Final Request

            request = new PackageRequest[]
                          { 
                              new AuthenticationRequest(true, string.Format("{0} {1}", securityToken, finalTicket))
                          };

            //Get response from the server!
            responses = this.socket.SendRangeAndBlock(request);

            #endregion

            #region Final Parse Responses If Any

            //Agora que temos as respostas vamos verificar se estão correctas!
            //Seja em quantidade, seja em 'qualidade'...

            if (responses.Count != 1)
            {
                throw new MessengerException("Unable to receive response from the server");
            }

            //Responses!
            AuthenticationAchievedResponse authenticatedGranted = responses[request[0].TransactionID].Response as AuthenticationAchievedResponse;

            authenticatedGranted.NotNull<AuthenticationAchievedResponse, MessengerProtocolException>("Unexpected response");

            #endregion

            //Hacker lingo!
            //We are in!

        }

        private void Authenticate(int attemptsP)
        {
            this.Authenticate(this.ConnectToDispathServer());
        }

        public void Authenticate(IPEndPoint host)
        {
            try
            {
                this.Authenticate(host, this.Attempts);
            }
            catch
            {
                this.socket = null;
                this.Credentials = null;
                throw;
            }
            this.socket.Reader.MessageReceived += this.EventParser;

        }

        public void Authenticate()
        {
            try
            {
                this.Authenticate(this.Attempts);
                this.Authenticated = true;
            }
            catch
            {
                this.socket = null;
                this.Credentials = null;
                this.Authenticated = false;
                throw;
            }
        }

        #endregion

        private static string Slack(string s, int length)
        {
            if (s.Length <= length)
            {
                return s;
            }
            return s.Substring(0, length);
        }

        private void RequestMembership()
        {
            this.socket.NotNull<MessengerSocket, MessengerAuthenticationException>("Authenticate first");

            MembershipRequest request = new MembershipRequest(this.Credentials["contacts.msn.com"].SecurityToken.Replace("&", "&amp;"));
            IList<Membership> membership = request.RequestMembership();

            this.Membership = membership;

        }

        private void RequestContacts()
        {
            this.socket.NotNull<MessengerSocket, MessengerAuthenticationException>("Authenticate first");

            AddressBookRequest request = new AddressBookRequest(this.Credentials["contacts.msn.com"].SecurityToken.Replace("&", "&amp;"), false);
            IList<Contact> contacts = request.RequestContacts();

            this.Contacts = contacts;


            //#region Output

            //lock (Console.Out)
            //{
            //    ConsoleColor backup = Console.BackgroundColor;
            //    ConsoleColor backup1 = Console.ForegroundColor;
            //    Console.ForegroundColor = ConsoleColor.Black;
            //    int i = 0;
            //    string s = string.Empty;
            //    if (Console.WindowWidth - 127 >= 0)
            //    {
            //        s = new string(' ', Console.WindowWidth - 127);
            //    }

            //    foreach (Contact item in contacts)
            //    {
            //        Console.BackgroundColor = (i % 2) == 0 ? ConsoleColor.Yellow : ConsoleColor.Cyan;
            //        Console.WriteLine(string.Format("{0,3} {1,22} DP| {2,32} QN| {3,64} P| {4}", i++, Slack(item.DisplayName, 12), Slack(item.QuickName, 32), Slack(item.PassportName, 64), s));
            //    }
            //    Console.ForegroundColor = backup1;
            //    Console.BackgroundColor = backup;
            //}

            //#endregion
            

        }

        private bool syncContacts = false;

        public void SyncronizeContacts()
        {
            if (this.Membership == null)
            {
                this.RequestMembership();
            }
            if (this.Contacts == null)
            {
                this.RequestContacts();
            }

            this.Contacts.NotNull();
            this.Membership.NotNull();

            foreach (Contact kvp in this.Contacts)
            {
                if (kvp.IsMessengerUser == false) continue;
                if (this.Membership.Count == 0) continue;
                kvp.MemberRole = (
                                                    from ms in this.Membership
                                                    where (ms.MemberRole == MemberRole.Allow || ms.MemberRole == MemberRole.Block) && ms.PassportName == kvp.PassportName
                                                    select ms.MemberRole
                                                ).First();
            }

            //Agora que temos ambos!
            //Vamos 'unir' os desgraçados!!!

            PackageRequest[] request = new PackageRequest[]
                                           {
                                               new PrivacySettingsRequest(true),
                                               new SyncronizeContactsRequest(this.Contacts),
                                               new EditUserPropertiesRequest(UserProperties.MFN, this.Account),                
                                           };

            Dictionary<int, PackageTransaction> response = this.socket.SendRangeAndBlock(request);

            syncContacts = true;

        }

        public void ChangeStatus(ClientStatus status)
        {
            this.socket.NotNull();

            const ClientCapabilities flags = ClientCapabilities.MSNC8
                                             | ClientCapabilities.DirectIM
                                             | ClientCapabilities.InkAsGif
                                             | ClientCapabilities.MultiPacketMessaging
                                             | ClientCapabilities.Search
                                             | ClientCapabilities.SharingFolders
                                             | ClientCapabilities.SIPInvitations
                                             | ClientCapabilities.VoiceClips
                                             | ClientCapabilities.WebCam
                                             | ClientCapabilities.Winks;

            PackageRequest r1 = new ChangeStatusRequest(status, flags, string.Empty);
            PackageRequest r2 = new InitialPresenceNotificationRequest(status, flags, string.Empty) {Account = this.Account};

            PackageRequest[] request = new PackageRequest[]
                                           {
                                               //r2,
                                               r1
                                           };

            Dictionary<int, PackageTransaction> response = this.socket.SendRangeAndBlock(request);

            //PackageResponse response = this.socket.SendAndBlock(request);

        }

        public void ChangePersonalMessage(string message)
        {
            this.socket.NotNull();

            PackageRequest request = new PersonalMessageRequest(message);

            PackageResponse response = this.socket.SendAndBlock(request);

        }

        public void PrivacySettings(bool allowBlocked)
        {
            this.socket.NotNull();

            PackageRequest request = new PrivacySettingsRequest(allowBlocked);

            PackageResponse response = this.socket.SendAndBlock(request);
        }

        public void InviteUserToChatSession(string account)
        {
            this.socket.NotNull();
            account.NotEmpty();

            PackageRequest request = new StartSwitchboardSessionRequest();

            NotificationServerRedirectionResponse response = this.socket.SendAndBlock(request) as NotificationServerRedirectionResponse;

            if (response == null)
            {
                Console.WriteLine("Invitation failed");
                return;
            }

            //Mixer swb = new Mixer(response.Host)

        }

        public bool ContactsSyncronized()
        {
            return this.syncContacts;
        }

    }

}