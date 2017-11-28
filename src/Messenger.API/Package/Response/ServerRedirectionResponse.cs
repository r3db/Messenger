using System;
using System.Net;
using Contracts;

namespace Messenger.API.Package.Response
{
    public sealed class DispatchServerRedirectionResponse : PackageResponse
    {
        public IPEndPoint Host { get; private set; }

        public DispatchServerRedirectionResponse(int trid, IPEndPoint host)
            : base(trid, PackageType.DispatchServerRedirection)
        {

            host.NotNull();

            this.Host = host;
        }

        public override string ToString()
        {
            return string.Format("XFR {0} NS {1} U D", base.TransactionID, this.Host);
        }

    }
}