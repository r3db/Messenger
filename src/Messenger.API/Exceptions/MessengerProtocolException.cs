using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace System.Net.Protocols.Messenger
{
    [Serializable]
    public class MessengerProtocolException : MessengerException
    {
        public MessengerProtocolException()
        { }

        public MessengerProtocolException(string message)
            : base(message)
        { }

        public MessengerProtocolException(string message, Exception inner)
            : base(message, inner)
        { }

        protected MessengerProtocolException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        { }
    }
}
