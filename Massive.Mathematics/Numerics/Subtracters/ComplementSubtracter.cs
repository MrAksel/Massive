using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Massive.Mathematics.Numerics.Subtracters
{
    class ComplementSubtracter : INaturalSubtracter
    {
        public void Subtract(Natural minuend, Natural subtrahend)
        {
            /*
             * If y is zero, return x
             * If x is larger than y, use Sub1
             * If x is smaller than y, would return negative
             * If x == y, set x to 0 and return x
             */

            if (subtrahend == 0)
            {
            }
            else if (minuend > subtrahend)
            {
                int digits = minuend.usedDigits;
                Natural.Complement(Natural.Add(Natural.Complement(minuend), subtrahend), digits);
            }
            else if (minuend < subtrahend)
            {
                throw new InvalidOperationException(Resources.Strings.WouldBeNegative);
            }
            else
            {
                minuend.Clear();
            }
        }

        public void Subtract(Natural minuend, Natural subtrahend, out Natural difference)
        {
            difference = minuend.Clone();
            Subtract(difference, subtrahend);
        }
    }
}
