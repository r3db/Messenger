using System;

namespace Messenger.API.Package.Response
{
    public sealed class SyncronizeContactsResponse : PackageResponse
    {
        public SyncronizeContactsResponse(int trid)
            : base(trid, PackageType.SyncronizeContacts)
        { }

        public override string ToString()
        {
            return string.Format("ADL {0} OK", base.TransactionID);
        }

    }
}