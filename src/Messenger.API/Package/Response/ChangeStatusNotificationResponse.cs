using System;

namespace Messenger.API.Package.Response
{
    public sealed class ChangeStatusNotificationResponse : AbstractPresenceNotificatonResponse
    {
        public ChangeStatusNotificationResponse(int trid, string account, string nickname, ClientStatus status, ClientCapabilities capabilities, NetworkId networkId, ObjectDescriptor descriptor)
            : base(trid, account, nickname, status, networkId, capabilities, descriptor, PackageType.ChangeStatusNotification)
        {}

        public ChangeStatusNotificationResponse(int trid, string account, string nickname, ClientStatus status, ClientCapabilities capabilities, NetworkId networkId)
            : this(trid, account, nickname, status, capabilities, networkId, null)
        { }

        public ChangeStatusNotificationResponse(int trid, string account, string nickname, ClientStatus status, ClientCapabilities capabilities)
            : this(trid, account, nickname, status, capabilities, (NetworkId)0, null)
        { }

        public ChangeStatusNotificationResponse(int trid, string account, string nickname, ClientStatus status)
            : this(trid, account, nickname, status, (ClientCapabilities)0, (NetworkId)0, null)
        { }

        public override string ToString()
        {
            if (this.Status == (ClientStatus)0)
            {
                return base.ToString(string.Empty);
            }
            return base.ToString("ILN");
        }

    }
}