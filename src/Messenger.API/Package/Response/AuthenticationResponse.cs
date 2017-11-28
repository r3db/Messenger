using System;
using Contracts;

namespace Messenger.API.Package.Response
{
    public sealed class AuthenticationResponse : PackageResponse
    {
        public bool IsSubsequent { get; private set; }
        public string Policy { get; private set; }
        public string Nonce { get; private set; }

        public AuthenticationResponse(int trid, string policy, string nonce)
            : base(trid, PackageType.Authentication)
        {
            policy.NotEmpty();
            nonce.NotEmpty();
            
            this.IsSubsequent = this.IsSubsequent;
            this.Policy = policy;
            this.Nonce = nonce;
        }

        public override string ToString()
        {
            return string.Format("USR SSO {0} {1} {2} {3}", base.TransactionID, this.IsSubsequent ? "S" : "I", this.Policy, this.Nonce);
        }
    }
}