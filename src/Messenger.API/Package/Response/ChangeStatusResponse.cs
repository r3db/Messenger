using System;

namespace Messenger.API.Package.Response
{
    public sealed class ChangeStatusResponse : AbstractPresenceNotificatonResponse
    {
        public ChangeStatusResponse(int trid, ClientStatus status, ClientCapabilities capabilities, ObjectDescriptor descriptor)
            : base(trid, string.Empty, string.Empty, status, (NetworkId)0, capabilities, descriptor, PackageType.ChangeStatus)
        {}

        public ChangeStatusResponse(int trid, ClientStatus status, ClientCapabilities capabilities)
            : this(trid, status, capabilities, null)
        { }

        public ChangeStatusResponse(int trid, ClientStatus status)
            : this(trid, status, (ClientCapabilities)0)
        { }
        
        public override string ToString()
        {
            return base.ToString("CHG");
        }

    }
}