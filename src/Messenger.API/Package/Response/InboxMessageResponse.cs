using System;
using System.Collections.Generic;
using System.Text;
using Contracts;
using Messenger.WebServices;

namespace Messenger.API.Package.Response
{
    public sealed class InboxMessageResponse : AbstractMessageResponse
    {
        public int Length { get; set; }
        public Dictionary<string, string> Parameters { get; set; }

        public InboxMessageResponse(Version mimeTypeVersion, string contentType, Dictionary<string, string> parameters, int length)
            : base(mimeTypeVersion, contentType)
        {
            parameters.NotNull();

            if (length < 0)
            {
                throw new ArgumentOutOfRangeException("length", "less than 0");
            }

            this.Parameters = parameters;
            this.Length = length;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            foreach (KeyValuePair<string, string> kvp in this.Parameters)
            {
                if (kvp.Key == "Mail-Data") continue;
                sb.AppendFormat("{0}: {1}{2}", kvp.Key, kvp.Value, Environment.NewLine);
            }
            if (sb.Length != 0)
            {
                sb.Remove(sb.Length - 2, 2);
            }

            //Mail-Data
            string formatedXml = this.Parameters["Mail-Data"].ToXmlFormat();

            return string.Format("MSG Hotmail Hotmail {0}{1}Mail-Data:{1}{2}{1}{3}{1}{5}", this.Length, Environment.NewLine, formatedXml, base.ToString(), Environment.NewLine, sb);
        }
    }
}