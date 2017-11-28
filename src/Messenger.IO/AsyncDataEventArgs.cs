using System;
using Contracts;
using System.Diagnostics;

namespace Messenger.IO
{
    // Done!
    public sealed class AsyncDataEventArgs : EventArgs
    {
        // Done!
        #region Internal Constants

        private const string Debug = "DEBUG";

        #endregion

        // Done!
        #region Internal Data

        private readonly byte[] message;

        #endregion

        // Done!
        #region .Ctor

        public AsyncDataEventArgs(byte[] packet, int identifier)
        {
            CtorContract(packet, identifier);

            this.message = packet;
            this.Identifier = identifier;
        }

        [Conditional(Debug)]
        private static void CtorContract(byte[] packet, int identifier)
        {
            packet.NotNull();
            packet.Length.BiggerOrEqualThan(1);
            identifier.BiggerOrEqualThan(0);
        }

        #endregion

        // Done!
        #region Properties

        public int Identifier { get; private set; }

        #endregion

        // Done!
        #region Methods

        public byte[] Message()
        {
            return this.message;
        }

        #endregion

    }
}
