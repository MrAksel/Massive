using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Digit = System.UInt32;
using Carry = System.UInt64;

namespace Massive.Mathematics.Numerics.Adders
{
    public class StandardAdder : INaturalAdder
    {
        public void Add(Natural x, Natural y)
        {
            int length = Math.Max(x.usedDigits, y.usedDigits);

            x.PushLength(length + 1);
            y.PushLength(length); // Expand so we know we can index those digits

            Digit carry = 0;
            for (int i = 0; i < length; i++)
            {
                Carry res = (Carry)x.digits[i] + y.digits[i] + carry;
                Digit val;
                if (res > Digit.MaxValue)
                {
                    carry = 1;
                    val = (Digit)(res - Digit.MaxValue - 1);
                }
                else
                {
                    carry = 0;
                    val = (Digit)res;
                }

                x.digits[i] = val;
            }
            x.digits[length] = carry;
            x.usedDigits = length + (int)carry; // If carry == 1 we used another extra digit

            x.PopLength();
            y.PopLength();
        }

        public void Add(Natural addend1, Natural addend2, out Natural sum)
        {
            sum = addend1.Clone();
            Add(sum, addend2);
        }
    }
}
