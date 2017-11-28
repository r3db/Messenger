using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace System.Net.Protocols.Messenger
{
    [Serializable]
    public class MessengerException : Exception
    {
        public MessengerException()
        { }

        public MessengerException(string message) : base(message)
        { }
        
        public MessengerException(string message, Exception inner)
            : base(message, inner)
        { }
        
        protected MessengerException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        { }
    }
}
