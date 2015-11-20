using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Digit = System.UInt32;
using Carry = System.UInt64;

namespace Massive.Mathematics.Numerics.Multipliers
{
    public class LongMultiplier : INaturalMultiplier
    {
        public void Multiply(Natural factor1, Natural factor2)
        {
            if (factor1 == 0 || factor2 == 0)
            {
                factor1.SetValue(Natural.Zero);
            }
            else if (factor2 == 1)
            {
                // factor1 = factor1    (jupp, that's the only optimization)
            }
            else
            {
                Natural product = Natural.Zero;

                for (int i = 0; i < factor1.usedDigits; i++)
                {
                    Digit d = factor1.digits[i];

                    Natural partial = MulDigit(factor2, d);
                    Natural.ShiftLeft(partial, i);

                    Natural.Add(product, partial);
                }

                factor1.SetValue(product);
            }
        }

        public void Multiply(Natural factor1, Natural factor2, out Natural product)
        {
            if (factor1 == 0 || factor2 == 0)
            {
                product = Natural.Zero;
            }
            else if (factor2 == 1)
            {
                product = factor1.Clone();
            }
            else
            {
                product = Natural.Zero;

                for (int i = 0; i < factor1.usedDigits; i++)
                {
                    Digit d = factor1.digits[i];

                    Natural partial = MulDigit(factor2, d);
                    Natural.ShiftLeft(partial, i);

                    Natural.Add(product, partial);
                }
            }
        }

        private static Natural MulDigit(Natural x, Digit y)
        {
            Natural total = 0;
            for (int i = 0; i < x.usedDigits; i++)
            {
                Natural partial = (Carry)x.digits[i] * y;
                Natural.ShiftLeft(partial, i);

                Natural.Add(total, partial);
            }

            return total;
        }

    }
}
