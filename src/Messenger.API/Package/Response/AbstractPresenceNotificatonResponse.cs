using System;
using System.Globalization;
using System.Web;
using Messenger.API.Utility;

namespace Messenger.API.Package.Response
{
    public abstract class AbstractPresenceNotificatonResponse : PackageResponse
    {
        public string Account { get; private set; }
        public string NickName { get; private set; }
        public NetworkId NetworkId { get; private set; }
        public ClientStatus Status { get; private set; }
        public ClientCapabilities Capabilities { get; private set; }
        public ObjectDescriptor Descriptor { get; private set; }

        protected AbstractPresenceNotificatonResponse(int trid, string account, string nick, ClientStatus status, NetworkId networkId, ClientCapabilities capabilities, ObjectDescriptor descriptor, PackageType packageType)
            : base(trid, packageType)
        {
            this.Account = account;
            this.NickName = nick;
            this.Status = status;
            this.NetworkId = networkId;
            this.Capabilities = capabilities;
            this.Descriptor = descriptor;

        }

        public bool IsCapabilityEnabled(ClientCapabilities cap)
        {
            return ((this.Capabilities & cap) == cap);
        }

        public string ToString(string command)
        {
            string empty = string.Empty;
            const string space = " ";

            return string.Format(
                CultureInfo.InvariantCulture,
                "{0} {1}{2} {3}{4}{5}{6}{7}{8}",
                command,
                base.TransactionID <= 0 ? empty : base.TransactionID + space,
                this.Status.GetUnderlyingValue(),
                string.IsNullOrEmpty(this.Account) ? empty : this.Account + space,
                this.NetworkId == 0 ? empty : (int)this.NetworkId + space,
                string.IsNullOrEmpty(this.NickName) ? empty : this.NickName + space,
                this.Capabilities == 0 ? empty : this.Capabilities + space,
                this.Descriptor == null ? empty : Environment.NewLine + this.Descriptor + space,
                string.IsNullOrEmpty(this.NickName) ? empty : Environment.NewLine + HttpUtility.UrlDecode(this.NickName) + space).Trim();

        }

    }
}