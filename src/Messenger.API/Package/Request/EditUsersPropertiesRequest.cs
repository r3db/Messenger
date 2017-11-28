using System;
using Contracts;

namespace Messenger.API.Package.Request
{
    public enum UserProperties
    {
        MBE, WWE, MFN
    }

    public sealed class EditUserPropertiesRequest : PackageRequest
    {
        public UserProperties Settings { get; private set; }
        public string Account { get; private set; }

        public EditUserPropertiesRequest(UserProperties settings, string account)
        {
            account.NotEmpty();

            this.Settings = settings;
            this.Account = account;

        }

        public override string ToString()
        {
            return string.Format("PRP {0} {1} {2}", base.TransactionID, this.Settings, this.Account);
        }

    }
}