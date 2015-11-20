using System;

[assembly: CLSCompliant(true)]

namespace Massive.Mathematics.Numerics
{
    public class Integer
    {
        private int sign;
        private Natural magnitude;

        #region Constructors

        public Integer()
        {
            sign = 0;
            magnitude = 0;
        }

        public Integer(Natural x)
        {
            sign = 0;
            magnitude = x;

            if (magnitude > 0)
                sign = 1;
        }
        
        public Integer(int value)
            : this()
        {
            sign = Math.Sign(value);
            magnitude = value;
        }

        [CLSCompliant(false)]
        public Integer(uint value)
            : this()
        {
            sign = (value > 0 ? 1 : 0);
            magnitude = value;
        }

        public Integer(long value)
            : this()
        {
            sign = Math.Sign(value);
            magnitude = value;
        }

        [CLSCompliant(false)]
        public Integer(ulong value)
            : this()
        {
            sign = (value > 0 ? 1 : 0);
            magnitude = value;
        }

        #endregion

        #region Unary arithmetic

        public static Integer operator -(Integer x)
        {
            Integer c = x.Clone();
            return Integer.Negate(c);
        }

        public static Integer operator ++(Integer x)
        {
            Integer c = x.Clone();
            return Integer.Increment(c);
        }

        public static Integer operator --(Integer x)
        {
            Integer c = x.Clone();
            return Integer.Decrement(c);
        }

        #endregion

        #region Arithmetic

        #region Addition

        public static Integer operator +(Integer x, Integer y)
        {
            Integer c = x.Clone();
            return Integer.Add(c, y);
        }

        #endregion

        #region Subtraction

        public static Integer operator -(Integer x, Integer y)
        {
            Integer c = x.Clone();
            return Integer.Subtract(c, y);
        }

        #endregion

        #region Multiplication

        public static Integer operator *(Integer x, Integer y)
        {
            Integer c = x.Clone();
            return Integer.Multiply(c, y);
        }

        #endregion

        #region Division

        public static Integer operator /(Integer x, Integer y)
        {
            Integer c = x.Clone();
            return Integer.Divide(c, y);
        }

        #endregion

        #region Modulus

        public static Integer operator %(Integer x, Integer y)
        {
            Integer c = x.Clone();
            return Integer.Modulus(c, y);
        }

        public static int operator %(Integer x, int y)
        {
            throw new NotImplementedException();
        }

        public static long operator %(Integer x, long y)
        {
            throw new NotImplementedException();
        }

        #endregion

        #endregion

        #region In-place arithmetic

        public static Integer Negate(Integer x)
        {
            x.sign *= -1;
            return x;
        }

        public static Integer Add(Integer x, Integer y)
        {
            if (y.sign == 0)
                return x;

            else if (x.sign == 0)
            {
                x.sign = y.sign;
                x.magnitude = y.magnitude.Clone();

                return x;   // Always update x instead of just returning y
            }
            else if (x.sign == y.sign)
            {
                x.magnitude += y.magnitude;
                return x;
            }
            else
            {
                // 5 + (-2) = 5 - (+2) = +3
                // 5 + (-7) = 5 - (+7) = -2

                // -5 + 2 = -3
                // -5 + 7 = +2

                if (x.magnitude > y.magnitude)
                {
                    Natural.Subtract(x.magnitude, y.magnitude);
                    return x;
                }
                else if (x.magnitude < y.magnitude)
                {
                    x.sign = -x.sign;
                    x.magnitude = y.magnitude - x.magnitude; // Can't use in-place subtract here, would create a reference to y's magnitude
                    return x;
                }
                else
                {
                    x.sign = 0;
                    x.magnitude = Natural.Zero;
                    return x;
                }
            }
        }


        public static Integer Subtract(Integer x, Integer y)
        {
            y.sign = -y.sign;
            Integer.Add(x, y);
            y.sign = -y.sign;

            return x; // x - y = x + (-y)
        }

        public static Integer Multiply(Integer x, Integer y)
        {
            Natural.Multiply(x.magnitude, y.magnitude);
            x.sign *= y.sign;
            return x;
        }

        public static Integer Divide(Integer x, Integer y)
        {
            if (y.sign == 0)
                throw new DivideByZeroException();

            Natural.Divide(x.magnitude, y.magnitude);
            x.sign /= y.sign;
            return x;
        }

        /// <summary>
        /// Returns the smallest (closest to zero) positive or negative remainder of the Euclidian division of x by y.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static Integer Modulus(Integer x, Integer y)
        {
            if (y.sign == 0)
                throw new DivideByZeroException();

            Integer pos = Integer.Remainder(x, y);
            Integer neg = pos - y;

            if (neg.magnitude < pos.magnitude)
                return x.SetValue(neg); // Set x to neg and return
            else
                return pos;             // x is already pos
        }

        /// <summary>
        /// Returns the smallest positive remainder of the Euclidian divison of x by y.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static Integer Remainder(Integer x, Integer y)
        {
            if (y.sign == 0)
                throw new DivideByZeroException();

            Natural.Modulus(x.magnitude, y.magnitude); // Return smalles positive remainder
            x.sign = 1;
            return x;
        }

        public static Integer Increment(Integer n)
        {
            return Integer.Add(n, 1);
        }

        public static Integer Decrement(Integer n)
        {
            return Integer.Subtract(n, 1);
        }

        #endregion

        #region Comparison

        #region Equality

        public static bool operator ==(Integer x, Integer y)
        {
            return (x.sign == y.sign && x.magnitude == y.magnitude);
        }

        [CLSCompliant(false)]
        public static bool operator ==(Integer x, uint y)
        {
            return (x.sign != -1 && x.magnitude == y);
        }
        [CLSCompliant(false)]
        public static bool operator ==(Integer x, ulong y)
        {
            return (x.sign != -1 && x.magnitude == y);
        }

        #endregion

        #region Inequality

        public static bool operator !=(Integer x, Integer y)
        {
            return (x.sign != y.sign || x.magnitude != y.magnitude);
        }

        [CLSCompliant(false)]
        public static bool operator !=(Integer x, uint y)
        {
            return (x.sign != y || x.magnitude != y);
        }
        [CLSCompliant(false)]
        public static bool operator !=(Integer x, ulong y)
        {
            return (x.sign != 1 || x.magnitude != y);
        }

        #endregion

        #region Less than

        public static bool operator <(Integer x, Integer y)
        {
            if (x.sign < y.sign)
                return true;
            if (y.sign < x.sign)
                return false;

            if (x.sign == -1) return x.magnitude > y.magnitude;

            return x.magnitude < y.magnitude;
        }

        [CLSCompliant(false)]
        public static bool operator <(Integer x, uint y)
        {
            if (x.sign == -1)
                return true;
            if (y == 0)
                return false;

            return x.magnitude < (Natural)y;
        }
        [CLSCompliant(false)]
        public static bool operator <(Integer x, ulong y)
        {
            if (x.sign == -1)
                return true;
            if (y == 0)
                return false;

            return x.magnitude < (Natural)y;
        }

        public static bool operator <=(Integer x, Integer y)
        {
            if (x.sign < y.sign)
                return true;

            if (x.sign > y.sign)
                return false;

            return x.magnitude < y.magnitude;
        }

        [CLSCompliant(false)]
        public static bool operator <=(Integer x, uint y)
        {
            if (x.sign == -1)
                return true;

            return x.magnitude < (Natural)y;
        }
        [CLSCompliant(false)]
        public static bool operator <=(Integer x, ulong y)
        {
            if (x.sign == -1)
                return true;

            return x.magnitude < (Natural)y;
        }

        #endregion

        #region Greater than

        public static bool operator >(Integer x, Integer y)
        {
            if (x.sign > y.sign)
                return true;

            if (x.sign < y.sign)
                return false;

            return x.magnitude > y.magnitude;
        }

        [CLSCompliant(false)]
        public static bool operator >(Integer x, uint y)
        {
            if (x.sign > y) // if x is positive and y is 0
                return true;

            if (x.sign == -1) // y can't be negative
                return false;

            return x.magnitude > (Natural)y;
        }
        [CLSCompliant(false)]
        public static bool operator >(Integer x, ulong y)
        {
            if (y == 0 && x.sign == 1) // if x is positive and y is 0
                return true;

            if (x.sign == -1) // y can't be negative
                return false;

            return x.magnitude > (Natural)y;
        }


        public static bool operator >=(Integer x, Integer y)
        {
            if (x.sign > y.sign)
                return true;

            if (x.sign < y.sign)
                return false;

            return x.magnitude >= y.magnitude;
        }

        [CLSCompliant(false)]
        public static bool operator >=(Integer x, uint y)
        {
            if (y == 0 && x.sign == 1)
                return true;

            if (x.sign == -1)
                return false;

            return x.magnitude == (Natural)y;
        }
        [CLSCompliant(false)]
        public static bool operator >=(Integer x, ulong y)
        {
            if (y == 0 && x.sign == 1)
                return true;

            if (x.sign == -1)
                return false;

            return x.magnitude == (Natural)y;
        }

        #endregion

        #endregion

        #region Implicit casting

        public static implicit operator Integer(Natural x)
        {
            return new Integer(x);
        }

        public static implicit operator Integer(byte x)
        {
            return new Integer((int)x);
        }

        public static implicit operator Integer(short x)
        {
            return new Integer((int)x);
        }

        public static implicit operator Integer(int x)
        {
            return new Integer(x);
        }

        public static implicit operator Integer(long x)
        {
            return new Integer(x);
        }

        #endregion

        #region Explicit casting

        public static explicit operator int (Integer x)
        {
            return (int)x.magnitude * x.sign;
        }

        public static explicit operator long (Integer x)
        {
            return (long)x.magnitude * x.sign;
        }

        #endregion

        #region Overloading

        public override bool Equals(object obj)
        {
            if (obj is Integer)
            {
                return this == (Integer)obj;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return magnitude.GetHashCode() ^ sign.GetHashCode();
        }

        public override string ToString()
        {
            return ToString(10);
        }

        public string ToString(int radix)
        {
            string s = magnitude.ToString(radix);
            if (sign == -1)
                return "-" + s;
            else
                return s;
        }

        #endregion

        #region Supplementary

        public Integer Clone()
        {
            Integer n = new Integer(magnitude.Clone());
            n.sign = this.sign;

            return n;
        }

        public Integer Clear()
        {
            this.magnitude.Clear();
            this.sign = 0;

            return this;
        }

        public Integer SetValue(Integer newVal)
        {
            this.magnitude.SetValue(newVal.magnitude);
            this.sign = newVal.sign;

            return this;
        }

        public Integer Swap(Integer other)
        {
            int s = sign;

            this.magnitude.Swap(other.magnitude);
            this.sign = other.sign;

            other.sign = s;

            return this;
        }

        #endregion

    }

}
