using System;
using Contracts;

namespace Messenger.API.Package.Response
{
    public sealed class PrincipalLeftChatResponse : PackageResponse
    {
        public string Account { get; private set; }
        public bool TimedOut { get; private set; }

        public PrincipalLeftChatResponse(string account, bool timedout)
            : base(-1, PackageType.PrincipalLeftChat)
        {
            account.NotEmpty();

            this.Account = account;
            this.TimedOut = timedout;
        }

        public override string ToString()
        {
            return string.Format("BYE {0}{1}", this.Account, this.TimedOut ? "1" : string.Empty);
        }

    }
}