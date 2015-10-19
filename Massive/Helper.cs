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
        public static byte[] ConvertUIntArrayToByteArray(uint[] v)
        {
            byte[] b = new byte[v.Length * 4];
            for (int i = 0; i < v.Length; i++)
            {
                uint u = v[i];
                int d = i * 4;
                b[d + 3] = (byte)(u & 0x000000FF); u >>= 8;
                b[d + 2] = (byte)(u & 0x000000FF); u >>= 8;
                b[d + 1] = (byte)(u & 0x000000FF); u >>= 8;
                b[d] = (byte)u;
            }
            return b;
        }

        // 12 34 56 78 = 0x12345678
        public static uint[] ConvertByteArrayToUIntArray(byte[] b)
        {
            int bL = b.Length + 4 - b.Length % 4;
            uint[] a = new uint[bL];

            for (int i = 0; i < bL; i++)
            {
                int d = i * 4;
                a[i] = (uint)b[d] << 24 | (uint)b[d + 1] << 18 |
                       (uint)b[d + 2] << 8 | (uint)b[d + 3];
            }

            return a;
        }
    }
}
