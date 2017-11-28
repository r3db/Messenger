using System;
using System.Xml;
using Contracts;
using Messenger.WebServices;

namespace Messenger.API.Package.Response
{
    public sealed class DeclarationResponse : PackageResponse
    {
        public int Length { get; private set; }
        public XmlDocument Payload { get; private set; }

        public DeclarationResponse(int trid, XmlDocument payload, int length)
            : base(trid, PackageType.Declaration)
        {
            payload.NotNull();

            if (trid != 0)
            {
                throw new ArgumentOutOfRangeException("trid", "excepted 0");
            }

            if (length < 0)
            {
                throw new ArgumentOutOfRangeException("length", "less than 0");
            }

            this.Payload = payload;
            this.Length = length;
        }

        public override string ToString()
        {
            return string.Format("GCF {0} {1}{2}{3}", base.TransactionID, this.Length, Environment.NewLine, XmlUtility.ToXmlFormat(this.Payload.InnerXml));
        }
    }
}