using System;

namespace Messenger.API.Package.Request
{
    public sealed class StartSwitchboardSessionRequest : PackageRequest
    {
        public override string ToString()
        {
            return string.Format("XFR {0} SB", base.TransactionID);
        }

    }
}