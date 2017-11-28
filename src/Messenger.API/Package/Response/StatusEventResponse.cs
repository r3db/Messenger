using System;
using Contracts;
using Messenger.API.Utility;

namespace Messenger.API.Package.Response
{
    public sealed class StatusEventResponse : PackageResponse
    {
        public string Account { get; private set; }
        public string NickName { get; private set; }
        public NetworkId NetworkId { get; private set; }
        public ClientStatus Status { get; private set; }
        public ClientCapabilities Capabilities { get; private set; }
        public string ObjectDescriptor { get; private set; }

        public StatusEventResponse(string account, string nickName, ClientStatus status, NetworkId networkId, ClientCapabilities capabilities, string objectDescriptor)
            : base(0, PackageType.StatusEvent)
        {
            account.NotEmpty();
            nickName.NotEmpty();

            this.Account = account;
            this.NickName = nickName;
            this.Status = status;
            this.NetworkId = networkId;
            this.Capabilities = capabilities;
            this.ObjectDescriptor = objectDescriptor;
        }

        public bool IsCapabilityEnabled(ClientCapabilities cap)
        {
            return ((this.Capabilities & cap) == cap);
        }

        public override string ToString()
        {
            return string.Format("NLN {0} {1} {2} {3}", base.TransactionID, this.Status.GetUnderlyingValue(), this.Account, this.NetworkId, this.NickName, (long)this.Capabilities, this.ObjectDescriptor);
        }

    }
}