using System;

namespace Messenger.WebServices
{
    public class Contact
    {
        public MemberRole   MemberRole              { get; set; }
        public Guid         ContactID               { get; set; }
        public ContactType  ContactType             { get; set; }
        public string       QuickName               { get; set; }
        public string       PassportName            { get; set; }
        public bool         IsPassportNameHidden    { get; set; }
        public string       DisplayName             { get; set; }
        public int          PUID                    { get; set; }
        public long         CID                     { get; set; }
        public bool         IsNotMobileVisible      { get; set; }
        public bool         IsMobileIMEnabled       { get; set; }
        public bool         IsMessengerUser         { get; set; }
        public bool         IsFavorite              { get; set; }
        public bool         IsSmtp                  { get; set; }
        public bool         HasSpace                { get; set; }
        public string       SpotWatchState          { get; set; }
        public DateTime     BirthDate               { get; set; }
        public string       PrimaryEmailType        { get; set; }
        public string       PrimaryLocation         { get; set; }
        public string       PrimaryPhone            { get; set; }
        public bool         IsPrivate               { get; set; }
        public string       Gender                  { get; set; }
        public string       TimeZone                { get; set; }
        public bool         Deleted                 { get; set; }
        public DateTime     LastChanged             { get; set; }

        public override string ToString()
        {
            return string.Format("{0} {1} {2}", this.QuickName, this.PassportName, this.DisplayName);
        }

    }
}
