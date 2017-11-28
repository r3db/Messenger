using System;
using Contracts;

namespace Messenger.API.Package.Response
{
    public sealed class AuthenticationAchievedResponse : PackageResponse
    {
        public bool Verified { get; private set; }
        public string Account { get; private set; }

        public AuthenticationAchievedResponse(int trid, bool verifed, string account)
            : base(trid, PackageType.AuthenticationAchieved)
        {
            account.NotEmpty();

            //Todo: Check Email!

            this.Verified = verifed;
            this.Account = account;
        }

        public override string ToString()
        {
            return string.Format("USR OK {0} {1} {2} 0", base.TransactionID, this.Account, this.Verified ? "1" : "0");
        }
    }
}