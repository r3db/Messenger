using System;
using System.Net;
using Contracts;

namespace Messenger.API.Package.Response
{
    public sealed class NotificationServerRedirectionResponse : PackageResponse
    {
        public IPEndPoint Host { get; private set; }
        public AuthorizationType Authorization { get; private set; }
        public string Ticket { get; private set; }
        //(1)
        public string DirectHost { get; private set; }
        public bool IsDirect { get; private set; }

        public NotificationServerRedirectionResponse(int trid, IPEndPoint host, AuthorizationType authorization, string ticket, string directHost, bool isDirect)
            : base(trid, PackageType.NotificationServerRedirection)
        {

            host.NotNull();
            ticket.NotEmpty();
            directHost.NotEmpty();

            this.Host = host;
            this.Authorization = authorization;
            this.Ticket = ticket;
            this.DirectHost = directHost;
            this.IsDirect = isDirect;

        }

        public override string ToString()
        {
            return string.Format("XFR {0} SB {1} {2} {3} U {4} {5}", base.TransactionID, this.Host, this.Authorization, this.Ticket, this.DirectHost, this.IsDirect == true ? 1 : 0);
        }

    }
}