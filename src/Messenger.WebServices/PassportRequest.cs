using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Xml;
using Contracts;

namespace Messenger.WebServices
{
    public sealed class PassportRequest : AbstractSoapRequest
    {
        // Done!
        #region Internal Constants

        private const string DefaultUri = "https://login.live.com/RST.srf";
        private static readonly string DefaultAction = string.Empty;

        #endregion

        // Done!
        #region Internal Data

        private readonly string username;
        private readonly string password;
        private readonly IDictionary<string, string> authorizationDomains;

        #endregion

        // Done!
        #region .Ctor

        // Done!
        public PassportRequest(string username, string password, string policy)
            : base(DefaultUri, DefaultAction)
        {
            username.NotEmpty();
            password.NotEmpty();
            policy.NotEmpty();

            this.username = username;
            this.password = password;

            this.authorizationDomains = new Dictionary<string, string>();
            this.InitializeAuthorizationDomains(policy);
        }

        private void InitializeAuthorizationDomains(string policy)
        {
            this.authorizationDomains.Add("messengerclear.live.com", policy);
            this.authorizationDomains.Add("messenger.msn.com", "?id=507");
            this.authorizationDomains.Add("contacts.msn.com", "MBI");
            this.authorizationDomains.Add("messengersecure.live.com", "MBI_SSL");
            this.authorizationDomains.Add("spaces.live.com", "MBI");
            this.authorizationDomains.Add("storage.msn.com", "MBI");
        }

        // Done!
        public PassportRequest(string username, string password, IDictionary<string, string> authDomains)
            : base(DefaultUri, DefaultAction)
        {
            username.NotEmpty();
            password.NotEmpty();
            authDomains.NotNull();
            authDomains.ElementNotNull();

            this.username = username;
            this.password = password;
            this.authorizationDomains = authDomains;
        }

        
        #endregion

        // Done!
        #region Methods

        // Done!
        protected override byte[] GetRequestContent()
        {
            StringBuilder sb = new StringBuilder();

            int counter = 1;
            foreach (KeyValuePair<string, string> domain in this.authorizationDomains)
            {
                sb.AppendFormat(CultureInfo.InvariantCulture, Properties.Resources.SecurityToken, counter++, domain.Key, domain.Value);
            }

            return Encoding.UTF8.GetBytes(string.Format(
                CultureInfo.InvariantCulture,
                Properties.Resources.AuthenticationRequest,
                this.username,
                this.password,
                sb));
        }

        // Done!
        public IDictionary<string, PassportCredentials> RequestCredentials()
        {
            Stream stream = this.GetResponseStream();

            XmlDocument xml = new XmlDocument();
            xml.Load(stream);

            XmlNodeList nodes = xml.GetElementsByTagName("wst:RequestSecurityTokenResponse");
            Dictionary<string, PassportCredentials> cred = new Dictionary<string, PassportCredentials>();

            foreach (XmlElement root in nodes)
            {
                //Extract domains!
                XmlNodeList child1 = root.GetElementsByTagName("wsp:AppliesTo");
                XmlNodeList child2 = ((XmlElement)child1[0]).GetElementsByTagName("wsa:EndpointReference");
                XmlNodeList child3 = ((XmlElement)child2[0]).GetElementsByTagName("wsa:Address");

                string domain = child3[0].InnerText;

                //Extract SecurityToken!
                XmlNodeList child4 = root.GetElementsByTagName("wst:RequestedSecurityToken");
                XmlNodeList child5 = ((XmlElement)child4[0]).GetElementsByTagName("wsse:BinarySecurityToken");

                string securityToken = (child5.Count >= 1) ? child5[0].InnerText : string.Empty;

                string binarySecret = string.Empty;
                XmlNodeList child6 = root.GetElementsByTagName("wst:RequestedProofToken");
                if (child6.Count >= 1)
                {
                    XmlNodeList child7 = ((XmlElement)child6[0]).GetElementsByTagName("wst:BinarySecret");
                    binarySecret = child7[0].InnerText;
                }

                cred.Add(domain, new PassportCredentials(domain, securityToken, binarySecret));

            }

            PrintResponseMessage(xml.InnerXml);

            return cred;
        }

        #endregion

    }
}
