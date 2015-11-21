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
                int cmp = dividend.CompareTo(modifiedDivisor);
                while (cmp > 0)
                {
                    Natural.BitShiftLeft(subquotient, 2); // Multiply both by 4 (seems to work okay)
                    Natural.BitShiftLeft(modifiedDivisor, 2);
                    cmp = dividend.CompareTo(modifiedDivisor);
                }
                if (cmp == 0) // Divisor equal to dividend!
                {
                    quotient = subquotient;
                    remainder = Natural.Zero;
                    return;
                }
                else if (cmp < 0) // Dividend < modifiedDivisor
                {
                    Natural.BitShiftRight(subquotient, 2); // Divide by 4 to get the right quotient
                    Natural.BitShiftRight(modifiedDivisor, 2);

                    // Dividend > modifiedDivisor
                }
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
