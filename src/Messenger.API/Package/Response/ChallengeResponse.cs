using System;
using Contracts;

namespace Messenger.API.Package.Response
{
    public sealed class ChallengeResponse : PackageResponse
    {
        public string Challenge { get; private set; }

        public ChallengeResponse(int trid, string challenge)
            : base(trid, PackageType.Challenge)
        {
            challenge.NotEmpty();

            this.Challenge = challenge;
        }

        public override string ToString()
        {
            return string.Format("CHL {0} {1}", base.TransactionID, this.Challenge);
        }

    }
}