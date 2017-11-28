using System;
using Contracts;

namespace Messenger.API.Package.Request
{
    public sealed class AuthenticationRequest : PackageRequest
    {
        public bool IsSubsequent {get; private set;}
        public string Parameter {get; private set;}

        public AuthenticationRequest(bool isSubsequent, string parameter)
        {
            parameter.NotEmpty();
            
            this.IsSubsequent = isSubsequent;
            this.Parameter = parameter;
        }

        public AuthenticationRequest(string parameter)
            : this(false, parameter)
        { }

        public override string ToString()
        {
            return string.Format("USR {0} SSO {1} {2}", base.TransactionID, this.IsSubsequent ? "S" : "I", this.Parameter);
        }

    }
}