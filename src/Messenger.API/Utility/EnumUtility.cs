using System;
using Messenger.API.Package.Response;

namespace Messenger.API.Utility
{
    public static class EnumUtility
    {
        public static string GetUnderlyingValue(this ClientStatus status)
        {
            switch (status)
            {
                case ClientStatus.Online:       return "NLN";
                case ClientStatus.Busy:         return "BSY";
                case ClientStatus.BeRightBack:  return "BRB";
                case ClientStatus.Away:         return "AWY";
                case ClientStatus.Idle:         return "IDL";
                case ClientStatus.OnThePhone:   return "PHN";
                case ClientStatus.OutForLunch:  return "LUN";
                case ClientStatus.Hidden:       return "HDN";
                default:                        throw new ArgumentException("status");
            }

        }

        public static ClientStatus ToClientStatus(this string status)
        {
            switch (status)
            {
                case "NLN": return ClientStatus.Online;
                case "BSY": return ClientStatus.Busy;
                case "BRB": return ClientStatus.BeRightBack;
                case "AWY": return ClientStatus.Away;
                case "IDL": return ClientStatus.Idle;
                case "PHN": return ClientStatus.OnThePhone;
                case "LUN": return ClientStatus.OutForLunch;
                case "HDN": return ClientStatus.Hidden;
                default: throw new ArgumentException("status");
            }

        }

    }
}