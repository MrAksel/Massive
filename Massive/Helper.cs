using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Massive
{
    public static class Helper
    {

        // 0x12345678 = 12 34 56 78
        public static byte[] ConvertUIntArrayToByteArrayBE(uint[] v)
        {
            byte[] b = new byte[v.Length * 4];
            for (int i = 0; i < v.Length; i++)
            {
                uint u = v[i];
                int d = i * 4;
                b[d + 3] = (byte)(u & 0x000000FF); u >>= 8;
                b[d + 2] = (byte)(u & 0x000000FF); u >>= 8;
                b[d + 1] = (byte)(u & 0x000000FF); u >>= 8;
                b[d + 0] = (byte)u;
            }
            return b;
        }
        // 0x12345678 = 78 56 34 12
        public static byte[] ConvertUIntArrayToByteArrayLE(uint[] v)
        {
            byte[] b = new byte[v.Length * 4];
            for (int i = 0; i < v.Length; i++)
            {
                uint u = v[i];
                int d = i * 4;
                b[d + 0] = (byte)(u & 0x000000FF); u >>= 8;
                b[d + 1] = (byte)(u & 0x000000FF); u >>= 8;
                b[d + 2] = (byte)(u & 0x000000FF); u >>= 8;
                b[d + 3] = (byte)u;
            }
            return b;
        }

        // 12 34 56 78 = 0x12345678
        public static uint[] ConvertByteArrayToUIntArrayBE(byte[] b)
        {
            int mod = b.Length % 4;
            int iL = b.Length / 4;
            int tL = iL + (mod > 0 ? 1 : 0);
            uint[] a = new uint[tL];

            for (int i = 0; i < iL; i++)
            {
                int d = i * 4;
                a[i] = ((uint)b[d] << 24) | ((uint)b[d + 1] << 16) |
                       ((uint)b[d + 2] << 8) | (uint)b[d + 3];
            }

            if (mod > 0) a[iL] |= (uint)b[iL * 4] << 24;
            if (mod > 1) a[iL] |= (uint)b[iL * 4 + 1] << 16;
            if (mod > 2) a[iL] |= (uint)b[iL * 4 + 2] << 8;

            return a;
        }

        // 12 34 56 78 = 0x78563412
        public static uint[] ConvertByteArrayToUIntArrayLE(byte[] b)
        {
            int mod = b.Length % 4;
            int iL = b.Length / 4;
            int tL = iL + (mod > 0 ? 1 : 0);
            uint[] a = new uint[tL];

            for (int i = 0; i < iL; i++)
            {
                int d = i * 4;
                a[i] = ((uint)b[d + 3] << 24) | ((uint)b[d + 2] << 16) |
                       ((uint)b[d + 1] << 8) | (uint)b[d + 0];
            }

            if (mod > 0) a[iL] |= (uint)b[iL * 4];
            if (mod > 1) a[iL] |= (uint)b[iL * 4 + 1] << 8;
            if (mod > 2) a[iL] |= (uint)b[iL * 4 + 2] << 16;

            return a;
        }
    }
}
