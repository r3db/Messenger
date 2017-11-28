using System;

namespace Messenger.WebServices
{
    public class Membership
    {
        public MemberRole   MemberRole              { get; set; }
        public MemberType   MemberType              { get; set; }
        public int          MembershipId            { get; set; }
        public string       Type                    { get; set; }
        public string       State                   { get; set; }
        public bool         Deleted                 { get; set; }
        public DateTime     LastChanged             { get; set; }
        public DateTime     JoinedDate              { get; set; }
        public DateTime     ExpirationDate          { get; set; }
        public string       PassportName            { get; set; }
        public bool         IsPassportNameHidden    { get; set; }
        public int          PassportId              { get; set; }
        public long         CID                     { get; set; }

        public override string ToString()
        {
            return string.Format("{0} {1}", this.MemberRole, this.PassportName);
        }

    }
}
