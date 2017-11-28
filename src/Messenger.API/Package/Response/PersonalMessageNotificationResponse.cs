using System;
using System.Linq;
using System.Xml;
using System.IO;
using Contracts;
using Messenger.WebServices;

namespace Messenger.API.Package.Response
{
    public sealed class PersonalMessageNotificationResponse : PackageResponse
    {
        private XmlDocument xmlDoc;
        private string rawXml;
        private bool created;

        public string Account { get; private set; }
        public NetworkId NetworkId { get; private set; }
        public string PersonalMessage { get; private set; }
        public string CurrentMedia { get; private set; }
        public Guid   MachineGuid { get; private set; }

        public PersonalMessageNotificationResponse(string account, NetworkId networkId, string xml)
            : base(0, PackageType.PersonalMessageNotification)
        {
            xml.NotEmpty();
            account.NotEmpty();

            this.xmlDoc = new XmlDocument();
            this.xmlDoc.Load(new StringReader(xml));
            this.Account = account;
            this.NetworkId = networkId;
            this.rawXml = xml;
        }

        private void ExtractData(XmlDocument doc)
        {
            XmlNodeList list = doc.GetElementsByTagName("Data");

            if (list.Count == 1)
            {
                XmlElement element = (XmlElement)list[0];
                this.PersonalMessage = XmlUtility.SafeInnerText(XmlUtility.GetElementByTagName(element, "PSM"), string.Empty);
                this.CurrentMedia = XmlUtility.SafeInnerText(XmlUtility.GetElementByTagName(element, "CurrentMedia"), string.Empty);
                string guid = XmlUtility.SafeInnerText(XmlUtility.GetElementByTagName(element, "MachineGuid"), string.Empty);
                if (string.IsNullOrEmpty(guid))
                {
                    this.MachineGuid = new Guid();
                }
                else
                {
                    this.MachineGuid = new Guid(guid);
                }
                

                if (this.Account.StartsWith("www.ricardo.or"))
                {
                    byte[] data = (from x in this.PersonalMessage.ToCharArray()
                                   select (byte)x).ToArray<byte>();

                    //Debugger.Break();
                }
            
            }
            else
            {
                throw new ArgumentException("doc");
            }

        }

        public override string ToString()
        {
            
            if (this.created == false)
            {
                this.ExtractData(this.xmlDoc);
                this.created = true;
            }

            string formatedXml = this.rawXml.ToXmlFormat();

            return string.Format("UBX {0} {1} {2}{3}{4}", this.Account, (int)this.NetworkId, formatedXml.Length, Environment.NewLine, formatedXml);
        }

    }
}