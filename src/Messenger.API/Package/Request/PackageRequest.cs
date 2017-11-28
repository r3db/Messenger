using System;
using System.Threading;

namespace Messenger.API.Package.Request
{
    public abstract class PackageRequest : AbstractPackage
    {
        private static int transactionID;
        public bool IsPayload { get; private set; }

        protected PackageRequest(int trid, bool isPayload)
            : base(trid, 0)
        {
            this.IsPayload = isPayload;
        }

        protected PackageRequest(int trid)
            : this(trid, false)
        { }

        protected PackageRequest(bool isPayload)
            : this(Interlocked.Increment(ref transactionID), isPayload)
        { }

        protected PackageRequest()
            : base(Interlocked.Increment(ref transactionID), 0)
        { }
    }
}