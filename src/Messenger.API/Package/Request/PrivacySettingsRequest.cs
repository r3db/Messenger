using System;

namespace Messenger.API.Package.Request
{
    public sealed class PrivacySettingsRequest : PackageRequest
    {
        public bool AllowBlocked {get; set;}

        public PrivacySettingsRequest(bool allowBlocked)
        {
            this.AllowBlocked = allowBlocked;
        }

        public PrivacySettingsRequest()
            : this(false)
        { }

        public override string ToString()
        {
            return string.Format("BLP {0} {1}", base.TransactionID, this.AllowBlocked ? "AL" : "BL");
        }

    }
}