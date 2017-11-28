using System;

namespace Messenger.API.Package.Response
{
    public sealed class PersonalMessageResponse : PackageResponse
    {
        public bool IsOK { get; private set; }

        public PersonalMessageResponse(int trid, bool isOK)
            : base(trid, PackageType.PersonalMessage)
        {
            this.IsOK = isOK;
        }

        public override string ToString()
        {
            return string.Format("UUX {0} {1}", base.TransactionID, this.IsOK ? "0" : "1");
        }

    }
}