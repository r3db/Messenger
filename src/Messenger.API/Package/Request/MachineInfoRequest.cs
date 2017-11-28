using System;
using System.Threading;
using Contracts;

namespace Messenger.API.Package.Request
{
    public sealed class MachineInfoRequest : PackageRequest
    {
        public int Locale { get; private set; }
        public string OSName { get; private set; }
        public Version OSVersion { get; private set; }
        public string Arquitecture { get; private set; }
        public string ClientName { get; private set; }
        public Version ClientVersion { get; private set; }
        public string Account { get; private set; }

        public MachineInfoRequest(int locale, string osName, Version osVersion, string arquitecture, string clientName, Version clientVersion, string account)
        {
            osName.NotEmpty();
            osVersion.NotNull();
            arquitecture.NotEmpty();
            clientName.NotEmpty();
            clientVersion.NotNull();
            account.NotEmpty();
            //Todo: Validate account!

            this.Locale = locale;
            this.OSName = osName;
            this.OSVersion = osVersion;
            this.Arquitecture = arquitecture;
            this.ClientName = clientName;
            this.ClientVersion = clientVersion;
            this.Account = account;
        }

        public MachineInfoRequest(string account)
            : this(Thread.CurrentThread.CurrentCulture.KeyboardLayoutId, "winnt", Environment.OSVersion.Version, "i386", "MSNMSGR", new Version(8, 5, 1302), account)
        { }

        public override string ToString()
        {
            string locale = string.Format("{0:x}", this.Locale);
            if (locale.Length == 3)
            {
                locale = "0" + locale;
            }

            return string.Format("CVR {0} 0x{1} {2} {3} {4} {5} {6} msmsgs {7}", base.TransactionID, locale, this.OSName, this.OSVersion, this.Arquitecture, this.ClientName, this.ClientVersion, this.Account);
        }

    }
}