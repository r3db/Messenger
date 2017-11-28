using System;
using Contracts;
using Messenger.API.Package.Request;
using Messenger.API.Package.Response;

namespace Messenger.API.Package
{
    public sealed class PackageTransaction
    {
        public PackageRequest  Request { get; set; }
        public PackageResponse Response { get; set; }

        public PackageTransaction(PackageRequest request, PackageResponse response)
        {
            request.NotNull();
            response.NotNull();

            if (request.TransactionID != response.TransactionID)
            {
                throw new ArgumentException("response", "The TransactionID property does not match");
            }

            this.Request = request;
            this.Response = response;

        }
    }
}