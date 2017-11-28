using System;
using System.Collections.Generic;
using System.Xml;
using Messenger.WebServices;

namespace Messenger.API.Package.Request
{
    public sealed class PersonalMessageRequest : PackageRequest
    {
        public string PersonalMessage { get; private set; }

        public Guid MachineGuid { get; private set; }

        public PersonalMessageRequest(string personalMessage)
            : base(true)
        {
            this.PersonalMessage = personalMessage;
        }

        private static string CreatePersonalMessage(string personalMessage, IEnumerable<string> currentMedia, Guid? machineGuid)
        {
            XmlDocument doc = new XmlDocument();

            XmlNode root = doc.CreateElement("Data");
            XmlNode psm = doc.CreateElement("PSM");
            psm.InnerText = personalMessage;
            XmlNode cm = doc.CreateElement("CurrentMedia");
            XmlNode mg = doc.CreateElement("MachineGuid");
            XmlNode ss = doc.CreateElement("SignatureSound");
            XmlNode ep = doc.CreateElement("EndpointData", doc.CreateFullAttribute("id", string.Format("{{{0}}}", new Guid())));
            XmlNode cp = doc.CreateElement("Capabilities");
            cp.InnerText = "2524762172:0";

            root.AppendChild(psm);
            root.AppendChild(cm);
            root.AppendChild(mg);
            ep.AppendChild(cp);
            root.AppendChild(ss);
            root.AppendChild(ep);
            doc.AppendChild(root);

            string formatedXml = XmlUtility.ToXmlFormat(doc.InnerXml);
            return formatedXml;
        }

        public override string ToString()
        {
            string formatedXml = XmlUtility.ToXmlFormat(CreatePersonalMessage(this.PersonalMessage, null, null));
            return string.Format("UUX {0} {1}\r\n{2}", base.TransactionID, base.Encoding.GetBytes(formatedXml).Length , formatedXml);
        }

    }
}