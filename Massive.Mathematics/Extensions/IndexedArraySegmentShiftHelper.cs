using Massive.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Massive.Mathematics.Extensions
{
    public static class IndexedArraySegmentShiftHelper
    {
        /// <summary>
        /// Note: Treats all integer types as least-significant-bit first, so a left shift will divide by two
        /// </summary>
        [CLSCompliant(false)]
        public static void BitShiftContentLeft(this IndexedArraySegment<uint> arr, long numBits)
        {
            // 0x00101010 01010101 11110000 00110011 -> shift left 1 bit ->
            // 0x10010101 00101010 11111000 00011001
            //      LSB ^               MSB ^
            // 0x01010100 10101010 00001111 11001100 // Looks like this with LSB first
            // 0x10101001 01010100 00011111 10011000

            int intBits = (int)(numBits % 32);
            if (numBits > 31)
            {
                arr.ShiftContentsLeft((int)(numBits / 32));
            }

            uint mask = (uint)Math.Pow(2, intBits) - 1;
            if (intBits != 0)
            {
                int end = arr.Offset + arr.Count - 1;
                uint[] array = arr.Array;

                for (int i = arr.Offset; i < end; i++)
                {

                    uint element = array[i];
                    uint nxtbits = array[i + 1] & mask;
                    element >>= intBits;
                    element |= nxtbits << (32 - intBits);
                    array[i] = element;

                    // array[i] = (array[i] >> intBits) | ((array[i + 1] & mask) << (32 - intBits));
                }
                array[end] >>= intBits;
            }
        }

        /// <summary>
        /// Note: Treats all integer types as least-significant-bit first, so a right shift will multiply by two
        /// </summary>
        /// <param name="arr"></param>
        /// <param name="numBits"></param>
        [CLSCompliant(false)]
        public static void BitShiftContentRight(this IndexedArraySegment<uint> arr, long numBits)
        {
            // 0x00101010 01010101 11110000 00110011 -> shift right 1 bit ->
            // 0x01010100 10101010 11100000 01100111
            //      LSB ^               MSB ^
            // 0x01010100 10101010 00001111 11001100 // Looks like this with LSB first
            // 0x00101010 01010101 00000111 11100110

            if (arr.Count == 0)
                return;

            if (arr.Count < numBits / 32)
            {
                uint[] array = arr.Array;
                for (int i = arr.Offset; i < arr.Offset + arr.Count; i++)
                    array[i] = 0;
                return;
            }

            int intBits = (int)(numBits % 32);
            if (numBits > 31)
            {
                arr.ShiftContentsRight((int)(numBits / 32));
            }

            uint mask = ((uint)Math.Pow(2, intBits) - 1) << (32 - intBits);
            if (intBits != 0)
            {
                int end = arr.Offset + arr.Count - 1;
                uint[] array = arr.Array;

                for (int i = end; i > arr.Offset; i--)
                {

                    uint element = array[i];
                    uint nxtbits = array[i - 1] & mask;
                    element <<= intBits;
                    element |= nxtbits >> (32 - intBits);
                    array[i] = element;

                    // array[i] = (array[i] << intBits) | ((array[i - 1] & mask) >> (32 - intBits));
                }
                array[arr.Offset] <<= intBits;
            }
        }
    }
}
