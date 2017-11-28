using System;

namespace Messenger.API.Package.Response
{
    public abstract class PackageResponse : AbstractPackage
    {
        protected PackageResponse(int trid, PackageType packageType)
            : base(trid, packageType)
        {
        }
    }
}