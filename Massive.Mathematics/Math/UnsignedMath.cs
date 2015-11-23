using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Massive.Mathematics
{
    public static class UnsignedMath
    {
        #region Base 2 Logarithm

        /// <summary>
        /// Returns base 2 logarithm of value. For value=0 this returns 0.
        /// </summary>
        [CLSCompliant(false)]
        public static int Log2(byte value)
        {
            int p = 0;
            while ((value >>= 1) > 0)
                p++;
            return p;
        }

        /// <summary>
        /// Returns base 2 logarithm of value. For value=0 this returns 0.
        /// </summary>
        [CLSCompliant(false)]
        public static int Log2(ushort value)
        {
            int p = 0;
            while ((value >>= 1) > 0)
                p++;
            return p;
        }

        /// <summary>
        /// Returns base 2 logarithm of value. For value=0 this returns 0.
        /// </summary>
        [CLSCompliant(false)]
        public static int Log2(uint value)
        {
            int p = 0;
            while ((value >>= 1) > 0)
                p++;
            return p;
        }

        /// <summary>
        /// Returns base 2 logarithm of value. For value=0 this returns 0.
        /// </summary>
        [CLSCompliant(false)]
        public static int Log2(ulong value)
        {
            int p = 0;
            while ((value >>= 1) > 0)
                p++;
            return p;
        }

        #endregion
    }
}
