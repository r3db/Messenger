using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;
using Contracts;

namespace Messenger.API.Package.Response
{
    public sealed class MachineInfoResponse : PackageResponse
    {
        public IEnumerable<Version> RecommendedVersion { get; private set; }
        public Version MinimumSafeVersion { get; private set; }
        public Uri LatestVersion { get; private set; }
        public Uri Information { get; private set; }

        public MachineInfoResponse(int trid, IEnumerable<Version> recommendedVersion, Version minimumSafeVersion, Uri latestVersion, Uri information)
            : base(trid, PackageType.MachineInfo)
        {
            recommendedVersion.ElementNotNull();
            minimumSafeVersion.NotNull();
            latestVersion.NotNull();
            information.NotNull();

            this.RecommendedVersion = recommendedVersion;
            this.MinimumSafeVersion = minimumSafeVersion;
            this.LatestVersion = latestVersion;
            this.Information = information;
        }

        private string GetVersionCorrect(Version version)
        {
            string build = version.Build.ToString();
            if (build.Length < 4)
            {
                build = "0" + build;
            }
            return string.Format("{0}.{1}.{2}", version.Major, version.Minor, build);
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            foreach (Version item in this.RecommendedVersion)
            {
                sb.Append(this.GetVersionCorrect(item) + " ");
            }

            if (Debugger.IsAttached)
            {
                return string.Format("CVR {0} {1}{2} {3} {4}", base.TransactionID, sb, this.GetVersionCorrect(this.MinimumSafeVersion), "http://...", "http://...");
            }
            else
            {
                return string.Format("CVR {0} {1}{2} {3} {4}", base.TransactionID, sb, this.GetVersionCorrect(this.MinimumSafeVersion), this.LatestVersion, this.Information);
            }
            
        }
    }
}