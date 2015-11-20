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
            if (dividend == divisor) // TODO Optimize with Natural.Compare(dividend, divisor)  (which returns 1, 0, or -1)
            {
                quotient = Natural.One;
                remainder = Natural.Zero;
            }
            else if (dividend < divisor)
            {
                quotient = Natural.Zero;
                remainder = dividend.Clone(); // Do we have to clone?
            }
            else // n > d
            {
                Natural modifiedDivisor = divisor.Clone();

                Natural subquotient = Natural.One;
                while (dividend >= modifiedDivisor) // TODO optimize to > (by checking inside loop)
                {
                    Natural.BitShiftLeft(subquotient, 1); // Multiply both by 2
                    Natural.BitShiftLeft(modifiedDivisor, 1);
                }
                Natural.BitShiftRight(subquotient, 1); // Divide by 2 to get the right quotient
                Natural.BitShiftRight(modifiedDivisor, 1);

                Natural newDividend = dividend - modifiedDivisor; // Optimize away the clone (put this in private function - clone up front)

                DivRem(newDividend, divisor, out quotient, out remainder); // Repeat division on difference between dividend and modifiedDivisor 

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
