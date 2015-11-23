using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Massive.Mathematics.Numerics.Dividers
{
    public class RecursiveDivider : INaturalDivider
    {
        public void Divide(Natural dividend, Natural divisor)
        {
            Natural quotient, remainder;
            DivRem(dividend, divisor, out quotient, out remainder);
            dividend.SetValue(quotient);
        }

        public void Divide(Natural dividend, Natural divisor, out Natural quotient)
        {
            Natural remainder;
            DivRem(dividend, divisor, out quotient, out remainder);
        }

        public void DivRem(Natural dividend, Natural divisor)
        {
            Natural quotient, remainder;
            DivRem(dividend, divisor, out quotient, out remainder);
            dividend.SetValue(quotient);
            divisor.SetValue(remainder);
        }

        public void DivRem(Natural dividend, Natural divisor, out Natural quotient, out Natural remainder)
        {
            InternalDivRem(dividend.Clone(), divisor, out quotient, out remainder);
        }

        private void InternalDivRem(Natural dividend, Natural divisor, out Natural quotient, out Natural remainder)
        {
            int dcmp = dividend.CompareTo(divisor);
            if (dcmp == 0)
            {
                quotient = Natural.One;
                remainder = Natural.Zero;
            }
            else if (dcmp < 0) // dividend < divisor
            {
                quotient = Natural.Zero;
                remainder = dividend.Clone(); // Do we have to clone?
            }
            else // dividend > divisor
            {
                Natural modifiedDivisor = divisor.Clone();
                Natural subquotient = Natural.One;

                long bitshifts = Natural.Log2(dividend) - Natural.Log2(modifiedDivisor); // Gets how many factors of two from divisor to dividend
                Natural.BitShiftLeft(modifiedDivisor, bitshifts); // Shifts divisor that many times (multiply by 2^bitshifts)
                Natural.BitShiftLeft(subquotient, bitshifts);

                int cmp = dividend.CompareTo(modifiedDivisor);
                if (cmp < 0) // Dividend < modifiedDivisors - shifted 1 too much
                {
                    Natural.BitShiftRight(subquotient, 1); // Divide by 2 to get the right quotient
                    Natural.BitShiftRight(modifiedDivisor, 1);
                }
                else if (cmp == 0) // Divisor equal to dividend!
                {
                    quotient = subquotient;
                    remainder = Natural.Zero;
                    return;
                }

                // dividend > modifiedDivisor
                Natural newDividend = Natural.Subtract(dividend, modifiedDivisor); // Optimize away the clone (put this in private function - clone up front)
                InternalDivRem(newDividend, divisor, out quotient, out remainder); // Repeat division on difference between dividend and modifiedDivisor 

                Natural.Add(quotient, subquotient); // Add the quotient we calculated in this iteration
            }
        }

        public void Remainder(Natural dividend, Natural divisor)
        {
            Natural quotient, remainder;
            DivRem(dividend, divisor, out quotient, out remainder);
            dividend.SetValue(remainder);
        }

        public void Remainder(Natural dividend, Natural divisor, out Natural remainder)
        {
            Natural quotient;
            DivRem(dividend, divisor, out quotient, out remainder);
        }
    }
}
