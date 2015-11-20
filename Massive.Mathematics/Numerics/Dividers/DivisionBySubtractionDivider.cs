using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Massive.Mathematics.Numerics.Dividers
{
    public class DivisionBySubtractionDivider : INaturalDivider
    {
        public void Divide(Natural dividend, Natural divisor)
        {
            Natural quotient, remainder;
            DivRem(dividend, divisor, out quotient, out remainder);
            dividend.SetValue(dividend);
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
            if (divisor == 0)
                throw new DivideByZeroException();

            dividend = dividend.Clone();

            quotient = 0;

            while (dividend >= divisor)
            {
                Natural.Subtract(dividend, divisor);
                Natural.Increment(quotient);
            }

            remainder = dividend;
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
