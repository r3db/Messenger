using System;
using Contracts;

namespace Messenger.API.Package.Response
{
    public sealed class RoosterResponse : PackageResponse
    {
        public int RoosterID { get; private set; }
        public int RoosterTotal { get; private set; }
        public string Account { get; private set; }
        public string Name { get; private set; }
        public ClientCapabilities Capabilities { get; private set; }

        public RoosterResponse(int trid, int roosterId, int roosterTotal, string account, string name, ClientCapabilities capabilities)
            : base(trid, PackageType.Rooster)
        {
            account.NotNull();
            name.NotEmpty();

            if (roosterId < 0)
            {
                throw new ArgumentOutOfRangeException("roosterId", "excepted 0");
            }

            if (roosterTotal < 0)
            {
                throw new ArgumentOutOfRangeException("roosterTotal", "less than 0");
            }

            this.RoosterID = roosterId;
            this.RoosterTotal = roosterTotal;
            this.Account = account;
            this.Name = name;
            this.Capabilities = capabilities;
        }

        public override string ToString()
        {
            return string.Format("IRO {0} {1} {2} {3} {4} {5}", base.TransactionID, this.RoosterID, this.RoosterTotal, this.Account, this.Name, (int)this.Capabilities);
        }
    }
}