using System;
using Contracts;

namespace Messenger.API.Package.Response
{
    public abstract class AbstractMessageResponse : PackageResponse
    {
        public Version MimeTypeVersion { get; set; }
        public string ContentType { get; set; }

        protected AbstractMessageResponse(Version mimeTypeVersion, string contentType)
            : base(0, PackageType.Message)
        {
            mimeTypeVersion.NotNull();
            contentType.NotEmpty();

            this.MimeTypeVersion = mimeTypeVersion;
            this.ContentType = contentType;
        }

        public override string ToString()
        {
            return string.Format("MIME-Version: {0}{1}Content-Type: {2}", this.MimeTypeVersion, Environment.NewLine, this.ContentType);
        }

    }
}