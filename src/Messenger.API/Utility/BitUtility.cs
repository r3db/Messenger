
using System;

namespace Messenger.API.Utility
{
    public static class BitUtility
    {
        private static readonly bool machineIsLittleEndian;

        static BitUtility()
        {
            machineIsLittleEndian = BitConverter.IsLittleEndian;
        }

        public static int ToInt32(byte[] data, int startIndex, bool dataIsLittleEndian)
        {
            if (machineIsLittleEndian == dataIsLittleEndian)
            {
                //the machine has the same endianess as the data, no need to swap bytes  
                return BitConverter.ToInt32(data, startIndex);
            }
            byte[] swap = GetSwappedByteArray(data, startIndex, 4);
            return BitConverter.ToInt32(swap, startIndex);
        }

        public static long ToInt64(byte[] data, int startIndex, bool dataIsLittleEndian)
        {
            if (machineIsLittleEndian == dataIsLittleEndian)
            {
                //the machine has the same endianess as the data, no need to swap bytes  
                return BitConverter.ToInt64(data, startIndex);
            }
            byte[] swap = GetSwappedByteArray(data, startIndex, 8);
            return BitConverter.ToInt64(swap, startIndex);
        }

        public static byte[] FromInt32(int val, bool littleEndian)
        {
            byte[] bytes = BitConverter.GetBytes(val);

            if (machineIsLittleEndian == littleEndian)
                return bytes;

            return GetSwappedByteArray(bytes, 0, 4);
        }

        public static byte[] FromInt64(long val, bool littleEndian)
        {
            byte[] bytes = BitConverter.GetBytes(val);

            if (machineIsLittleEndian == littleEndian)
                return bytes;

            return GetSwappedByteArray(bytes, 0, 8);
        }

        private static byte[] GetSwappedByteArray(byte[] data, int startIndex, int length)
        {
            byte[] swap = new byte[length];
            while (--length >= 0)
                swap[length] = data[startIndex + length];
            return swap;
        }
    }
}