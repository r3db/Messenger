using System;
using System.IO;
using System.Threading;
using Contracts;

namespace Messenger.IO
{
    public sealed class AsyncStreamReader : IAsyncStreamReader
    {
        // Done!
        #region Events

        public event EventHandler<AsyncDataEventArgs> DataReceived;

        #endregion

        // Done!
        #region Public Constants

        public const int MinBufferSize = 512;
        public const int MinimumWaitTime = 100;

        #endregion

        // Done!
        #region Internal Data

        private int identifier;
        private int wait;

        private readonly Stream stream;
        private readonly byte[] buffer;

        private readonly AsyncCallback callbackRead;
        private readonly object locker = new object();

        #endregion

        // Done!
        #region .Ctor

        // Done!
        public AsyncStreamReader(Stream stream, int bufferSize, int wait)
        {
            stream.NotNull();
            stream.CanRead.Equals(true);
            
            this.buffer = new byte[Math.Max(bufferSize, MinBufferSize)];
            this.stream = stream;
            this.Wait = wait;
            this.callbackRead = this.OnReadComplete;
        }

        // Done!
        public AsyncStreamReader(Stream stream, int bufferSize)
            : this(stream, bufferSize, MinimumWaitTime)
        {
        }

        // Done!
        public AsyncStreamReader(Stream stream)
            : this(stream, 4096, MinimumWaitTime)
        {
        }

        #endregion

        // Done!
        #region Properties

        // Done!
        public int Wait
        {
            get { return this.wait; }
            set { this.wait = Math.Max(value, 0); }
        }

        #endregion

        // Todo: Remove Thread.Sleep(wait); from ReadComplete(IAsyncResult ar)
        #region AsyncStream Reader

        // Done!
        private void BeginDataReceive()
        {
            if (this.IsListening)
            {
                this.stream.BeginRead(buffer, 0, buffer.Length, callbackRead, null);
            }
        }

        // Done!
        private void OnReadComplete(IAsyncResult ar)
        {
            ReadComplete(ar);
        }

        // Todo: Remove Thread.Sleep(wait);
        private void ReadComplete(IAsyncResult ar)
        {
            if (!ar.IsCompleted)
            {
                throw new InvalidOperationException("Should never happen!");
            }

            lock (this.locker)
            {
                int bytesRead = this.stream.EndRead(ar);

                // Normally is empty only at the end of the session!
                // So we will ignore it!
                if (bytesRead <= 0) return;

                if (this.DataReceived != null)
                {
                    int id = identifier++;
                    byte[] copy = new byte[bytesRead];
                    Array.ConstrainedCopy(this.buffer, 0, copy, 0, bytesRead);
                    this.Invoke(new AsyncDataEventArgs(copy, id));
                }

            }

            // Note: Seems to be the optimal value.
            // Note: Without this wait it doesn't work, discover why with UnitTesting
            Thread.Sleep(wait);
            this.BeginDataReceive();

        }

        // Todo: Conver to Private Again!
        public void Invoke(AsyncDataEventArgs ar)
        {
            this.DataReceived.BeginInvoke(this, ar, null, null);
        }

        #endregion

        // Done!
        #region Interface IAsyncStreamReader

        // Done!
        public bool IsListening { get; set; }

        // Done!
        public void StartListening()
        {
            this.IsListening = true;
            this.BeginDataReceive();
        }

        #endregion

        // Done!
        #region IDisposable

        // Done!
        public void Dispose()
        {
            if (this.stream == null) return;

            this.stream.Close();
            this.stream.Dispose();
        }

        #endregion

    }
}