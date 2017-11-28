using System;
using System.Collections.Generic;
using System.Text;
using Contracts;
using Messenger.WebServices;

namespace Messenger.API.Package.Response
{
    public sealed class OfflineMessageResponse : AbstractMessageResponse
    {
        public int Length { get; set; }
        public Dictionary<string, string> Parameters { get; set; }

        public OfflineMessageResponse(Version mimeTypeVersion, string contentType, Dictionary<string, string> parameters, int length)
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
                if (kvp.Key == "Mail-Data")
                {
                    sb.AppendFormat("{0}:{2}{1}{2}", kvp.Key, kvp.Value.ToXmlFormat(), Environment.NewLine);
                }
                else
                {
                    sb.AppendFormat("{0}: {1}{2}", kvp.Key, kvp.Value, Environment.NewLine);
                }            
            }
            if (sb.Length != 0)
            {
                sb.Remove(sb.Length - 2, 2);
            }

            return string.Format("MSG Hotmail Hotmail {0}{1}{2}{3}{4}", this.Length, Environment.NewLine, base.ToString(), Environment.NewLine, sb);
        }
    }
}