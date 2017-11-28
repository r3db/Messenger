using System;
using System.Collections.Generic;

namespace Messenger.API.Package.Response
{
    public sealed class AcceptedChatInvitationResponse : PackageResponse
    {
        public IEnumerable<Version> RecommendedVersion { get; private set; }
        public Version MinimumSafeVersion { get; private set; }
        public Uri LatestVersion { get; private set; }
        public Uri Information { get; private set; }

        public AcceptedChatInvitationResponse(int trid)
            : base(trid, PackageType.ChatInvitation)
        { }

        public override string ToString()
        {
            return string.Format("ANS {0} OK", base.TransactionID);
        }
    }
}