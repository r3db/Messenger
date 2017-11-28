using System;
using Messenger.API.Package.Response;
using Messenger.API.Utility;

namespace Messenger.API.Package.Request
{
    public sealed class ChangeStatusRequest : PackageRequest
    {
        public ClientStatus Status { get; private set; }
        public ClientCapabilities Capabilities { get; private set; }
        public string ObjectDescriptor { get; private set; }

        public ChangeStatusRequest(ClientStatus status, ClientCapabilities capabilities, string objectDescriptor)
        {
            this.Status = status;
            this.Capabilities = capabilities;
            this.ObjectDescriptor = objectDescriptor;
        }

        public bool IsCapabilityEnabled(ClientCapabilities cap)
        {
            return ((this.Capabilities & cap) == cap);
        }

        public override string ToString()
        {
            //return string.Format("CHG {0} {1} {2}", base.TransactionID, this.Status.GetUnderlyingValue(), (int)this.Capabilities + " %3Cmsnobj%20Creator%3D%22www.ricardo.org%40hotmail.com%22%20Type%3D%223%22%20SHA1D%3D%22WliJzSCj1p0%2B9%2BBOKoKSC%2FcHKMk%3D%22%20Size%3D%2224040%22%20Location%3D%220%22%20Friendly%3D%22TQBvAHQAbwBjAGkAYwBsAG8AIAB0AG8AZABvAC0AbwAtAHQAZQByAHIAZQBuAG8AAAA%3D%22%2F%3E");
            return string.Format("CHG {0} {1} {2} {3}", base.TransactionID, Status.GetUnderlyingValue(), 2253180964, ObjectDescriptor);
            //return string.Format("CHG {0} {1} {2} {3}", base.TransactionID, Status.GetUnderlyingValue(), (int)Capabilities, ObjectDescriptor);
        }

    }
}