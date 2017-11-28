using System;
using System.Text;
using System.Security.Cryptography;
using Contracts;

namespace Messenger.API.Package.Request
{
    public sealed class ChallengeRequest : PackageRequest
    {
        public string Challenge { get; private set; }
        public string MD5dDigest { get; private set; }

        public ChallengeRequest(string challenge)
            : base(true)
        {
            challenge.NotEmpty();

            this.Challenge = challenge;
        }

        #region Compute Challenge

        private const string _productID = "ILTXC!4IXB5FB*PX";
        private const string _productKey = "PROD0119GSJUC$18";

        public static string GetChallengeResponse(string challenge)
        {
            string hash = CreateMD5HexString(challenge + _productID);
            int[] smallChunks = CreateSmallChunks(hash);
            string chal = challenge + _productKey;
            chal = chal.PadRight(chal.Length + (8 - chal.Length % 8), '0');
            int[] bigChunks = CreateBigChunks(chal);
            long key = CalculateKey(bigChunks, smallChunks);

            return (String.Format("{0:x}", Convert.ToInt64(hash.Substring(0, 16), 16) ^ key).PadLeft(16, '0') +
                    String.Format("{0:x}", Convert.ToInt64(hash.Substring(16, 16), 16) ^ key).PadLeft(16, '0'));
        }

        private static string CreateMD5HexString(string str)
        {
            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
            byte[] hash = md5.ComputeHash(Encoding.UTF8.GetBytes(str));

            StringBuilder sb = new StringBuilder();

            foreach (byte b in hash)
            {
                sb.Append(String.Format("{0:x2}", b));
            }

            return sb.ToString();
        }

        private static int[] CreateSmallChunks(string hash)
        {
            int[] smallChunks = new int[4];

            for (int i = 0; i < 4; i++)
            {
                smallChunks[i] = Convert.ToInt32("0x" + HexSwap(hash.Substring(0, 8)), 16);
                smallChunks[i] = smallChunks[i] & 0x7FFFFFFF;

                hash = hash.Remove(0, 8);
            }

            return (smallChunks);
        }

        private static int[] CreateBigChunks(string challenge)
        {
            int[] bigChunks = new int[challenge.Length / 4];

            for (int i = 0; i < bigChunks.Length; i++)
            {
                string chunk = challenge.Substring(i * 4, 4);
                string hex = String.Empty;

                foreach (char c in chunk)
                    hex = hex + String.Format("{0:x}", (int)c);

                hex = HexSwap(hex);

                bigChunks[i] = Convert.ToInt32("0x" + hex, 16);
            }

            return (bigChunks);
        }

        private static string HexSwap(string str)
        {
            StringBuilder sb = new StringBuilder();

            if (str.Length % 2 > 0)
            {
                str = "0" + str;
            }

            for (int i = 0; i < str.Length; i += 2)
            {
                sb.Insert(0, str.Substring(i, 2));
            }

            return sb.ToString();
        }

        private static long CalculateKey(int[] bigChunks, int[] smallChunks)
        {
            long high = 0;
            long low = 0;

            for (int i = 0; i < bigChunks.Length; i = i + 2)
            {
                long tmp = bigChunks[i];
                tmp = (tmp * 242854337);
                tmp = tmp % 0x7FFFFFFF;
                tmp += high;
                tmp = smallChunks[0] * tmp + smallChunks[1];
                tmp = tmp % 0x7FFFFFFF;

                high = bigChunks[i + 1];
                high = (high + tmp) % 0x7FFFFFFF;
                high = smallChunks[2] * high + smallChunks[3];
                high = high % 0x7FFFFFFF;

                low = low + high + tmp;
            }

            high = (high + smallChunks[1]) % 0x7FFFFFFF;
            string hexHigh = HexSwap(String.Format("{0:x}", high));
            high = Convert.ToInt64("0x" + hexHigh, 16);

            low = (low + smallChunks[3]) % 0x7FFFFFFF;

            string hexLow = HexSwap(
                String.Format("{0:x}", low).PadLeft(8, '0')
                );

            low = Convert.ToInt64("0x" + hexLow, 16);

            long key = (high << 32) + low;

            return (key);
        }

        #endregion

        public override string ToString()
        {
            string payload = GetChallengeResponse(this.Challenge);

            return string.Format("QRY {0} {1} {2}\r\n{3}", base.TransactionID, _productKey, payload.Length, payload);
        }

    }
}