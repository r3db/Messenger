using System;
using Contracts;

namespace Messenger.API.Package.Request
{
    public sealed class ChatInvitationRequest : PackageRequest
    {
        public string Account {get; private set;}

        public ChatInvitationRequest(string account)
        {
            account.NotEmpty();

            this.Account = account;
        }

        public override string ToString()
        {
            return string.Format("CAL {0} {1}", base.TransactionID, this.Account);
        }

    }
}