using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.IO;

using Contracts;

namespace Messenger.WebServices
{
    internal static class XmlUtility
    {
        public static XmlDocument LoadXml(string xml)
        {
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xml);
            return doc;
        }

        public static string ToXmlFormat(this string xml)
        {
            try
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    using (XmlTextWriter writer = new XmlTextWriter(ms, Encoding.Unicode))
                    {
                        writer.Formatting = Formatting.Indented;
                        XmlDocument xmlDoc = new XmlDocument();
                        xmlDoc.LoadXml(xml);
                        xmlDoc.WriteContentTo(writer);
                        writer.Flush();
                        ms.Flush();
                        ms.Position = 0;
                        using (StreamReader sr = new StreamReader(ms))
                        {
                            return sr.ReadToEnd();
                        }
                    }
                }
            }
            catch
            {
                return xml;
            }
        }

        public static XmlNode GetElementByTagName(this XmlElement element, string tagName, int index)
        { 
            XmlNodeList list = element.GetElementsByTagName(tagName);
            return (index + 1) > list.Count ? null : list[index];
        }

        public static XmlNode GetElementByTagName(this XmlElement element, string tagName)
        {
            return GetElementByTagName(element, tagName, 0);
        }

        public static string SafeInnerText(this XmlNode node, string defaultParam)
        {
            if (node == null)
            {
                return defaultParam;
            }
            return node.InnerText;
        }

        public static string SafeInnerText(this XmlNode node)
        {
            return SafeInnerText(node, string.Empty);
        }

        public static string SafeInnerXml(this XmlNode node, string defaultParam)
        {
            if (node == null)
            {
                return defaultParam;
            }
            return node.InnerXml;
        }

        public static string SafeInnerXml(this XmlNode node)
        {
            return SafeInnerXml(node, string.Empty);
        }

        public static XmlAttribute CreateFullAttribute(this XmlDocument document, string name, object value)
        {
            document.NotNull();

            XmlAttribute attribute = document.CreateAttribute(name);
            attribute.Value = value.ToString();
            return attribute;
        }

        public static XmlNode CreateElement(this XmlDocument document, string name, IEnumerable<XmlAttribute> attributes)
        {
            document.NotNull();

            XmlNode node = document.CreateElement(name);

            foreach(XmlAttribute item in attributes)
            {
                node.Attributes.Append(item);
            }

            return node;

        }

        public static XmlNode CreateElement(this XmlDocument document, string name, XmlAttribute attribute)
        {
            return CreateElement(document, name, new XmlAttribute[] { attribute });
        }

    }
}