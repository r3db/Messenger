using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Serialization;

namespace System.Net.Protocols.Messenger
{
    [Serializable]
    public class MessengerAuthenticationException : MessengerException
    {
        public MessengerAuthenticationException()
        { }

        public MessengerAuthenticationException(string message)
            : base(message)
        { }
        
        public MessengerAuthenticationException(string message, Exception inner) 
            : base(message, inner)
        { }
        
        protected MessengerAuthenticationException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        { }
    }
}
