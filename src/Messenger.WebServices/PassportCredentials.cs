using System;
using Contracts;

namespace Messenger.WebServices
{
    public struct PassportCredentials
    {
        // Done!
        #region Internal Data

        public string Domain        { get; set; }
        public string SecurityToken { get; set; }
        public string BinarySecret  { get; set; }

        #endregion

        // Done!
        #region .Ctor

        public PassportCredentials(string domain, string securityToken, string binarySecret) : this()
        {
            domain.NotEmpty();

            this.Domain = domain;
            this.SecurityToken = securityToken;
            this.BinarySecret = binarySecret;
        }

        #endregion

    }
}
