using System;

namespace Messenger.API.Package.Response
{
    public sealed class PrivacySettingsResponse : PackageResponse
    {
        public bool AllowBlocked { get; set; }

        public PrivacySettingsResponse(int trid, bool allowBlocked)
            : base(trid, PackageType.PrivacySettings)
        {
            this.AllowBlocked = allowBlocked;
        }

        public PrivacySettingsResponse(int trid)
            : this(trid, false)
        { }

        public override string ToString()
        {
            return string.Format("BLP {0} {1}", base.TransactionID, this.AllowBlocked == true ? "AL" : "BL");
        }

    }
}