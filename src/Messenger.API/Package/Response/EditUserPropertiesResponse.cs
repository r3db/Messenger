using System;
using Contracts;
using Messenger.API.Package.Request;

namespace Messenger.API.Package.Response
{
    public sealed class EditUserPropertiesResponse : PackageResponse
    {
        public UserProperties Settings { get; private set; }
        public string Account { get; private set; }

        public EditUserPropertiesResponse(int trid, UserProperties settings, string account)
            : base(trid, PackageType.EditUserProperties)
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