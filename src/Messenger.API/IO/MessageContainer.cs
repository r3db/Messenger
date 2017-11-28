using System;
using System.Collections.Generic;

namespace Messenger.API.IO
{
    // Done!
    internal struct MessageContainer
    {
        public string       Message         { get; set; }
        public string       Remaining       { get; set; }
        public int          TransactionID   { get; set; }
        public List<string> Parameters      { get; set; }
    }
}
