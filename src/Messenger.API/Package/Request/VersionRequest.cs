using System;
using System.Collections.Generic;
using System.Text;
using Contracts;

namespace Messenger.API.Package.Request
{
    public sealed class VersionRequest : PackageRequest
    {
        public IEnumerable<string> Protocols { get; private set; }

        public VersionRequest(IEnumerable<string> protocols)
        {
            protocols.ElementNotNull();

            this.Protocols = protocols;
        }

        public VersionRequest()
            : this(new string[] { "MSNP15", "MSNP14", "MSNP13" })
        { }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            foreach (string item in this.Protocols)
            {
                sb.Append(item + " ");
            }

            return string.Format("VER {0} {1}CVR0", base.TransactionID, sb);
        }

    }
}