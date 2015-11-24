using Massive.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Carry = System.UInt64;
using Digit = System.UInt32;

namespace Massive.Mathematics.Numerics.Subtracters
{
    public class CarrySubtracter : INaturalSubtracter
    {
        public void Subtract(Natural minuend, Natural subtrahend)
        {
            Natural difference;
            Subtract(minuend, subtrahend, out difference);
            minuend.SetValue(difference);
        }

        public void Subtract(Natural minuend, Natural subtrahend, out Natural difference)
        {
            int cmp = minuend.CompareTo(subtrahend);
            if (cmp < 0)
            {
                throw new InvalidOperationException(Resources.Strings.WouldBeNegative);
            }
            else if( cmp == 0)
            {
                difference = Natural.Zero;
            }
            else
            {
                IndexedArraySegment<Digit> digits = new Digit[minuend.usedDigits];
                int carry = 0;
                int msd = 0;
                for (int i = 0; i < minuend.usedDigits; i++)
                {
                    long diff = (long)minuend.digits[i] - subtrahend.digits[i] - carry;
                    if (diff < 0) // Need to borrow from next
                    {
                        diff = diff + Digit.MaxValue + 1;
                        carry = 1;
                        digits[i] = (Digit)diff;
                    }
                    else if (diff > 0)
                    {
                        digits[i] = (Digit)diff;
                        carry = 0;
                        msd = i;
                    }
                }
                difference = new Natural(digits, msd + 1);
            }
        }
    }
}
