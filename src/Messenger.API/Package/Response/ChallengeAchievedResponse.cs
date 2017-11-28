using System;

namespace Messenger.API.Package.Response
{
    public sealed class ChallengeAchievedResponse : PackageResponse
    {
        public ChallengeAchievedResponse(int trid)
            : base(trid, PackageType.ChallengeAchieved)
        { }

        public override string ToString()
        {
            return string.Format("QRY {0}", base.TransactionID);
        }

    }
}