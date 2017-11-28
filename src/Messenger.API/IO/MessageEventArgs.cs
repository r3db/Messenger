using System;
using Messenger.API.Package.Response;

namespace Messenger.API.IO
{
    public sealed class MessageEventArgs : EventArgs
    {
        public PackageResponse Package { get; private set; }

        public int Identifier { get; private set; }

        public MessageEventArgs(PackageResponse package)
        {
            this.Package = package;
        }
    }
}