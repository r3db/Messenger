using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Xml;
using Contracts;

namespace Messenger.WebServices
{
    public sealed class MembershipRequest : AbstractSoapRequest
    {
        // Done!
        #region Internal Constants

        private const string DefaultUri = "http://contacts.msn.com/abservice/SharingService.asmx";
        private const string DefaultAction = "http://www.msn.com/webservices/AddressBook/FindMembership";

        #endregion

        // Done!
        #region Internal Data


        private DateTime    LastChange  { get; set; }
        private string      TickToken   { get; set; }
        private bool        IsMigration { get; set; }
        private bool        FullRequest { get; set; }

        #endregion

        // Done!
        #region .Ctor

        public MembershipRequest(string tickToken, bool fullRequest, bool isMigration, DateTime lastChange)
            : base(DefaultUri, DefaultAction)
        {
            tickToken.NotEmpty();

            this.TickToken = tickToken;
            this.FullRequest = fullRequest;
            this.IsMigration = isMigration;
            this.LastChange = lastChange;
        }

        public MembershipRequest(string tickToken)
            : this(tickToken, false, false, DateTime.MinValue)
        {
        }

        #endregion

        // Done!
        #region Methods

        // Done!
        protected override byte[] GetRequestContent()
        {
            string lastChangeReq = string.Empty;

            if (this.LastChange != DateTime.MinValue)
            {
                lastChangeReq = string.Format(
                    CultureInfo.InvariantCulture,
                    Properties.Resources.MembershipLastChange,
                    this.FullRequest.ToString().ToLowerInvariant(),
                    this.LastChange.ToString(TextUtility.ISO8601Format)
                    );
            }

            // We will ask "this" to the server!
            string membershipRequest = string.Format(
                CultureInfo.InvariantCulture,
                Properties.Resources.MembershipRequest,
                this.IsMigration.ToString().ToLowerInvariant(),
                this.TickToken,
                lastChangeReq
                );

            return Encoding.UTF8.GetBytes(membershipRequest);

        }

        // Done!
        public IList<Membership> RequestMembership()
        {
            Stream stream = this.GetResponseStream();
            stream.NotNull();

            StreamReader sr = new StreamReader(stream);
            string xml = sr.ReadToEnd();

            PrintResponseMessage(xml);

            return GetMembershipFromXmlDocument(XmlUtility.LoadXml(xml));

        }

        // Done!
        private static IList<Membership> GetMembershipFromXmlDocument(XmlDocument doc)
        {
            IList<Membership> membership = new List<Membership>(1000);

            XmlNodeList nodes = doc.GetElementsByTagName("FindMembershipResult/Services/Service/Memberships/Membership");
            
            foreach (XmlElement node in nodes)
            {
                MemberRole memberRole = (MemberRole)Enum.Parse(typeof(MemberRole), node.GetElementByTagName("MemberRole").SafeInnerText());
                
                foreach (XmlElement subNode in node.GetElementsByTagName("Member"))
                {
                    #region Extract Data From Xml


                    MemberType memberType = (MemberType)Enum.Parse(typeof(MemberType), subNode.Attributes["xsi:type"].Value);
                    int membershipId = int.Parse(subNode.GetElementByTagName("MembershipId").SafeInnerText());
                    string type = subNode.GetElementByTagName("Type").SafeInnerText();
                    string state = subNode.GetElementByTagName("State").SafeInnerText();
                    bool deleted = bool.Parse(subNode.GetElementByTagName("Deleted").SafeInnerText());
                    //DateTime lastChangedA = DateTime.ParseExact(subNode.GetElementByTagName("LastChanged").SafeInnerText(), TextUtility.ISO8601Format, CultureInfo.InvariantCulture);
                    //DateTime joinedDateA = DateTime.ParseExact(subNode.GetElementByTagName("JoinedDate").SafeInnerText(), TextUtility.ISO8601Format, CultureInfo.InvariantCulture);
                    //DateTime expirationDateA = DateTime.ParseExact(subNode.GetElementByTagName("ExpirationDate").SafeInnerText(), TextUtility.ISO8601Format, CultureInfo.InvariantCulture);
                    string passportName = subNode.GetElementByTagName("PassportName").SafeInnerText();
                    bool isPassportNameHidden = bool.Parse(subNode.GetElementByTagName("IsPassportNameHidden").SafeInnerText("false"));
                    int passportId = int.Parse(subNode.GetElementByTagName("PassportId").SafeInnerText("-1"));
                    long cid = long.Parse(subNode.GetElementByTagName("CID").SafeInnerText("-1"));

                    #endregion

                    membership.Add(new Membership
                    {
                        CID = cid,
                        Deleted = deleted,
                        //ExpirationDate = expirationDateA,
                        IsPassportNameHidden = isPassportNameHidden,
                        //JoinedDate = joinedDateA,
                        //LastChanged = lastChangeA,
                        MemberRole = memberRole,
                        MembershipId = membershipId,
                        MemberType = memberType,
                        PassportId = passportId,
                        PassportName = passportName,
                        State = state,
                        Type = type
                    });

                }

            }

            return membership;

        }

        #endregion

    }
}
