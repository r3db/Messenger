using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using Contracts;
using System.Globalization;

namespace Messenger.WebServices
{
    public sealed class AddressBookRequest : AbstractSoapRequest
    {
        // Done!
        #region Internal Constants

        private const string DefaultUri = "https://contacts.msn.com/abservice/abservice.asmx";
        private const string DefaultAction = "http://www.msn.com/webservices/AddressBook/ABFindAll";

        #endregion

        // Done!
        #region Internal Data

        public DateTime TimeStamp   { get; set; }
        public bool     FullRequest { get; set; }
        public bool     IsMigration { get; set; }
        public string   TicketToken { get; set; }

        #endregion

        // Done!
        #region .Ctor

        public AddressBookRequest(string ticketToken, bool isMigration, bool fullRequest, DateTime timeStamp)
            : base(DefaultUri, DefaultAction)
        {
            ticketToken.NotEmpty();

            this.TicketToken = ticketToken;
            this.IsMigration = isMigration;
            this.TimeStamp = timeStamp;
            this.FullRequest = fullRequest;
        }

        public AddressBookRequest(string ticketToken, bool isMigration)
            : this(ticketToken, isMigration, false, DateTime.MinValue)
        { }

        #endregion

        // Done!
        #region Methods

        // Done!
        protected override byte[] GetRequestContent()
        {
            // We will ask "this" to the server!
            string contactRequest = string.Format(
                CultureInfo.InvariantCulture,
                Properties.Resources.ContactRequest,
                IsMigration.ToString().ToLowerInvariant(),
                TicketToken,
                FullRequest.ToString().ToLowerInvariant(),
                this.TimeStamp.ToString(TextUtility.ISO8601Format));

            return Encoding.UTF8.GetBytes(contactRequest);

        }

        // Done!
        public IList<Contact> RequestContacts()
        {
            Stream stream = this.GetResponseStream();
            stream.NotNull();

            StreamReader sr = new StreamReader(stream);
            string xml = sr.ReadToEnd();

            PrintResponseMessage(xml);

            return GetContactsFromXmlDocument(XmlUtility.LoadXml(xml));

        }

        // Done!
        private static IList<Contact> GetContactsFromXmlDocument(XmlDocument doc)
        {
            IList<Contact> contacts = new List<Contact>(500);

            XmlNodeList nodes = doc.GetElementsByTagName("Contact");

            foreach (XmlElement node in nodes)
            {
                #region Extract Data From Xml

                Guid contactID = new Guid(node.GetElementByTagName("contactId").SafeInnerText("00000000-0000-0000-0000-000000000000"));
                ContactType contactType = (ContactType)Enum.Parse(typeof(ContactType), node.GetElementByTagName("contactType").InnerText);
                string quickName = node.GetElementByTagName("quickName").SafeInnerText();
                string passportName = node.GetElementByTagName("passportName").SafeInnerText();
                bool isPassportNameHidden = bool.Parse(node.GetElementByTagName("IsPassportNameHidden").SafeInnerText("false"));
                string displayName = node.GetElementByTagName("displayName").SafeInnerText();
                int puid = int.Parse(node.GetElementByTagName("puid").SafeInnerText("-1"));
                long cid = long.Parse(node.GetElementByTagName("CID").SafeInnerText("0"));
                bool isNotMobileVisible = bool.Parse(node.GetElementByTagName("IsNotMobileVisible").SafeInnerText("false"));
                bool isMobileIMEnabled = bool.Parse(node.GetElementByTagName("isMobileIMEnabled").SafeInnerText("false"));
                bool isMessengerUser = bool.Parse(node.GetElementByTagName("isMessengerUser").SafeInnerText("false"));
                bool isFavorite = bool.Parse(node.GetElementByTagName("isFavorite").SafeInnerText("false"));
                bool isSmtp = bool.Parse(node.GetElementByTagName("isSmtp").SafeInnerText("false"));
                bool hasSpace = bool.Parse(node.GetElementByTagName("hasSpace").SafeInnerText("false"));
                string spotWatchState = node.GetElementByTagName("spotWatchState").SafeInnerText();
                //DateTime birthdate = DateTime.Parse(node.GetElementByTagName("birthdate").SafeInnerText());
                string primaryEmailType = node.GetElementByTagName("primaryEmailType").SafeInnerText();
                string primaryLocation = node.GetElementByTagName("PrimaryLocation").SafeInnerText();
                string primaryPhone = node.GetElementByTagName("PrimaryPhone").SafeInnerText();
                bool isPrivate = bool.Parse(node.GetElementByTagName("IsPrivate").SafeInnerText("true"));
                string gender = node.GetElementByTagName("gender").SafeInnerText();
                string timeZone = node.GetElementByTagName("TimeZone").SafeInnerText();
                bool fDeleted = bool.Parse(node.GetElementByTagName("fDeleted").SafeInnerText("true"));
                //DateTime lastChange = DateTime.Parse(node.GetElementByTagName("lastChange").SafeInnerText());

                #endregion

                contacts.Add(new Contact
                {
                    ContactType = contactType,
                    QuickName = quickName,
                    DisplayName = displayName,
                    PassportName = passportName,
                    Gender = gender,
                    CID = cid,
                    ContactID = contactID,
                    Deleted = fDeleted,
                    HasSpace = hasSpace,
                    IsFavorite = isFavorite,
                    IsMessengerUser = isMessengerUser,
                    IsMobileIMEnabled = isMobileIMEnabled,
                    IsNotMobileVisible = isNotMobileVisible,
                    IsPassportNameHidden = isPassportNameHidden,
                    IsPrivate = isPrivate,
                    IsSmtp = isSmtp,
                    PrimaryEmailType = primaryEmailType,
                    PrimaryLocation = primaryLocation,
                    PrimaryPhone = primaryPhone,
                    PUID = puid,
                    SpotWatchState = spotWatchState,
                    TimeZone = timeZone
                });

            }
            return contacts;

        }

        #endregion

    }

}
