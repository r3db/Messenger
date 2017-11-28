using System;
using System.Web;
using Messenger.WebServices;

namespace Messenger.API
{
    public sealed class ObjectDescriptor
    {
        private readonly string data;

        public ObjectDescriptor(string data)
        {
            this.data = data;
        }

        public override string ToString()
        {
            return XmlUtility.ToXmlFormat(HttpUtility.UrlDecode(this.data));
        }

    }
}
