using System;
using System.Collections.Generic;
using System.Text;
using Contracts;

namespace Messenger.API.Package.Response
{
    public sealed class VersionResponse : PackageResponse
    {
        public IEnumerable<string> Protocols { get; private set; }

        public VersionResponse(int trid, IEnumerable<string> protocols)
            : base(trid, PackageType.Version)
        {
            protocols.ElementNotNull();
            protocols.CannotContainElement("CVR0");

            this.Protocols = protocols;
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            foreach (string item in this.Protocols)
            {
                sb.Append(item + " ");
            }

            return string.Format("VER {0} {1}CVR0", base.TransactionID, sb.ToString());
        }

    }
}