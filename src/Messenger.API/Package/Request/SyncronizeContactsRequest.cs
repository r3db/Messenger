using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using Contracts;
using Messenger.WebServices;

namespace Messenger.API.Package.Request
{
    public sealed class SyncronizeContactsRequest : PackageRequest
    {
        public IEnumerable<Contact> Contacts { get; private set; }

        public SyncronizeContactsRequest(IEnumerable<Contact> contacts)
            : base(true)
        {
            contacts.ElementNotNull();

            this.Contacts = contacts;
        }

        private static string CreateMailingList(IEnumerable<Contact> contacts)
        {
            var q = (from x in contacts
                     where x.IsMessengerUser == true
                     //orderby x.PassportName.Substring(x.PassportName.IndexOf('@') + 1).ToLowerInvariant() ascending
                     //orderby x.PassportName.Substring(x.PassportName.Substring(x.PassportName.IndexOf('@') + 1).IndexOf("."))
                     orderby x.PassportName.Substring(0, x.PassportName.IndexOf('@')).ToLowerInvariant() ascending
                     group x
                         by x.PassportName.Substring(x.PassportName.IndexOf('@') + 1)).ToList();

            XmlDocument doc = new XmlDocument();

            XmlNode root = doc.CreateElement("ml", doc.CreateFullAttribute("l", "1"));

            foreach (var group in q)
            {
                XmlNode domain = doc.CreateElement("d", doc.CreateFullAttribute("n", group.Key.ToLowerInvariant()));
                foreach (var item in group)
                {
                    XmlAttribute[] attributes = new XmlAttribute[]
                                                    {
                                                        doc.CreateFullAttribute("n", item.PassportName.Substring(0, item.PassportName.IndexOf('@')).ToLowerInvariant()),
                                                        doc.CreateFullAttribute("l", 1 + ((item.MemberRole == MemberRole.Allow) ? 2 : 4)),
                                                        doc.CreateFullAttribute("t", 1)
                                                    };

                    XmlNode contact = doc.CreateElement("c", attributes);
                    domain.AppendChild(contact);
                }
                root.AppendChild(domain);
            }

            doc.AppendChild(root);


            string formatedXml = XmlUtility.ToXmlFormat(doc.InnerXml);
            return formatedXml;
        }

        public override string ToString()
        {  
            string formatedXml = XmlUtility.ToXmlFormat(CreateMailingList(this.Contacts));
            return string.Format("ADL {0} {1}\r\n{2}", base.TransactionID, formatedXml.Length, formatedXml);
        }

    }
}