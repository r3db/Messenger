using System;
using System.Text;
using Contracts;

namespace Messenger.API.Package
{
    public abstract class AbstractPackage
    {
        #region Internal Constants

        private static readonly Encoding DefaultEncoding = new UTF8Encoding(false, false);

        #endregion

        #region Internal Data

        public PackageType PackageType { get; protected set; }
        public int TransactionID { get; protected set; }
        public Encoding Encoding { get; protected set; }

        #endregion

        #region .Ctor

        protected AbstractPackage(int trid, Encoding encoding, PackageType packageType)
        {
            encoding.NotNull();
            trid.BiggerOrEqualThan(0);

            this.TransactionID = trid;
            this.Encoding = encoding;
            this.PackageType = packageType;
        }

        protected AbstractPackage(int trid, PackageType packageType)
            : this(trid, DefaultEncoding, packageType)
        {
        }

        #endregion

    }
}