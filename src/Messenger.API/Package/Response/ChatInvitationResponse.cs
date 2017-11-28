using System;
using System.Net;
using Contracts;

namespace Messenger.API.Package.Response
{
    public sealed class ChatInvitationResponse : PackageResponse
    {
        public int SessionID { get; private set; }
        public IPEndPoint Host { get; private set; }
        public AuthorizationType Authorization { get; private set; }
        public string Ticket { get; private set; }
        public string Passport { get; private set; }
        public string Name { get; private set; }
        //(1)
        public string DirectHost { get; private set; }
        public bool DirectConnection { get; private set; }
        

        public ChatInvitationResponse(int sessionId, IPEndPoint host, AuthorizationType authorization, string ticket, string passport, string name, string directHost, bool directConnection)
            : base(0, PackageType.ChatInvitation)
        {
            host.NotNull();
            ticket.NotEmpty();
            passport.NotEmpty();
            name.NotEmpty();
            directHost.NotEmpty();

            this.SessionID = sessionId;
            this.Host = host;
            this.Authorization = authorization;
            this.Ticket = ticket;
            this.Passport = passport;
            this.Name = name;
            this.DirectHost = directHost;
            this.DirectConnection = directConnection;

        }

        public override string ToString()
        {
            //RNG 411794213 207.46.27.18:1863 CKI 158218235.17512347 www.ricardo.org@hotmail.com Ricardo%20Alternativo U messenger.msn.com 1
            return string.Format("RNG {0} {1} {2} {3} {4} {5} U {6} {7}", this.SessionID, this.Host, this.Authorization, this.Ticket, this.Passport, this.Name, this.DirectHost, this.DirectConnection ? 1 : 0);
        }

    }
}