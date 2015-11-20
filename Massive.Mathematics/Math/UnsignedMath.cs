using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Massive.Mathematics
{
    public static class UnsignedMath
    {
        #region Find highest bit

        [CLSCompliant(false)]
        public static int HighestBitSetPosition(byte value)
        {
            int p = 0;
            while ((value >>= 1) > 0)
                p++;
            return p;
        }

        [CLSCompliant(false)]
        public static int HighestBitSetPosition(ushort value)
        {
            int p = 0;
            while ((value >>= 1) > 0)
                p++;
            return p;
        }

        [CLSCompliant(false)]
        public static int HighestBitSetPosition(uint value)
        {
            int p = 0;
            while ((value >>= 1) > 0)
                p++;
            return p;
        }

        [CLSCompliant(false)]
        public static int HighestBitSetPosition(ulong value)
        {
            int p = 0;
            while ((value >>= 1) > 0)
                p++;
            return p;
        }

        #endregion
    }
}
