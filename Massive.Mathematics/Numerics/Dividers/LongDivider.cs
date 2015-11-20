using Massive.Collections;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Digit = System.UInt32;
using Carry = System.UInt64;

namespace Massive.Mathematics.Numerics.Dividers
{
    public class LongDivider : INaturalDivider
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
            throw new NotImplementedException();

            /*
            if (divisor == Natural.Zero)
                throw new DivideByZeroException();

            IndexedArraySegment<Digit> divisorDigits = divisor.digits.ToArray();
            List<Digit> digits = new List<Digit>();

            while (dividend > divisor) // TODO Optimize this away (because we're comparing which is larger in the inner loop too)
            {
                // Take enough digits to be larger than divisor
                IndexedArraySegment<Digit> divisor_part = divisorDigits.GetSegment(0, divisor.usedDigits); // Is this the right endianness?
                for (int i = divisor.usedDigits - 1; divisor >= 0; divisor--)
                {
                    if (divisor.digits[i] > divisor_part[i])
                    {
                        divisor_part.ExpandWindow(divisor_part.Count + 1);
                        break;
                    }
                }

                Natural msdigits = new Natural(divisor_part);
                // Divide msdigits by divisor and place quotient (single digit) where it belongs
                // Remainder is left over to next iteration as the new divisor_part
                // Still have to expand divisor_part so that its larger than divisor

                // We have a guarantee that msdigits / divisor will be a quotient with magnitude 1 (one digit) (because maths)
                // Still, we might have 0xFFFFFFFF / 2 = 0x7FFFFFFF, so division by subtraction is ineffective...

                Natural digit, rem;
                DivModSlow(msdigits, divisor, out digit, out rem); // This method also places remainder in dividend (msdigits), so digits in divisor_part are updated

                digits.Add(digit.digits[0]);
            }

            remainder = dividend;
            digits.Reverse();
            quotient = new Natural(digits.ToArray());
            */
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
