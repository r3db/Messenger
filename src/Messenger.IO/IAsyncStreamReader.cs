using System;

namespace Messenger.IO
{
    // Done!
    public interface IAsyncStreamReader : IDisposable
    {
        void StartListening();
        bool IsListening { get; set; }
    }
}
