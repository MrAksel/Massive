using Massive.Collections;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Numerics;
using System.Text;
using System.Threading;
using Carry = System.UInt64;
using Digit = System.UInt32;


namespace Massive.Mathematics.Numerics
{
    public class Natural : IComparable, IFormattable, IConvertible, IComparable<Natural>, IEquatable<Natural>
    {
        private const string WouldBeNegative = "Operation would return a negative value.";

        private static int default_number_size = 4;
        public static int DefaultNumberSize
        {
            get
            {
                return default_number_size;
            }
            set
            {
                if (value < 1)
                    throw new InvalidOperationException("DefaultNumberSize must be larger than zero");

                int size = RoundToPow2(value);
                Interlocked.Exchange(ref default_number_size, size);
            }
        }

        private int usedDigits;
        private Stack<int> lengths;

        private IndexedArraySegment<Digit> digits;

        public static Natural Zero
        {
            get
            {
                return new Natural();
            }
        }

        public static Natural One
        {
            get
            {
                return new Natural(1);
            }
        }

        public static Natural Base
        {
            get
            {
                return Natural.Increment(new Natural(Digit.MaxValue));
            }
        }

        #region Constructors

        public Natural()
        {
            digits = new Digit[default_number_size];

            usedDigits = 1;
            lengths = new Stack<int>();
            lengths.Push(0);
        }

        public Natural(int value)
            : this()
        {
            if (value < 0)
                throw new InvalidOperationException(WouldBeNegative);

            digits[0] = (uint)value;
            usedDigits = 1;
        }

        [CLSCompliant(false)]
        public Natural(uint value)
            : this()
        {
            digits[0] = value;
            usedDigits = 1;
        }

        public Natural(long value)
            : this()
        {
            if (value < 0)
                throw new InvalidOperationException(WouldBeNegative);

            ulong v = (ulong)value;

            Grow(2);

            digits[0] = (uint)(v & 0x00000000FFFFFFFF);
            digits[1] = (uint)((v & 0xFFFFFFFF00000000) >> 32);
            usedDigits = (digits[1] > 0 ? 2 : 1);
        }

        [CLSCompliant(false)]
        public Natural(ulong value)
            : this()
        {
            Grow(2);

            digits[0] = (uint)(value &  0x00000000FFFFFFFF);
            digits[1] = (uint)((value & 0xFFFFFFFF00000000) >> 32);
            usedDigits = (digits[1] > 0 ? 2 : 1);
        }

        [CLSCompliant(false)]
        public Natural(Digit[] digits, int nDigits = -1)
        {
            this.digits = digits;
            lengths = new Stack<int>();
            lengths.Push(0);

            if (nDigits < 1)
                NotifyUpdate();
            else
                this.usedDigits = nDigits;
        }

        #endregion

        #region Unary arithmetic

        public static Natural operator ++(Natural x)
        {
            return x + 1;
        }

        public static Natural operator --(Natural x)
        {
            if (x == 0)
                throw new InvalidOperationException(WouldBeNegative);
            return x - 1;
        }

        #endregion

        #region Arithmetic

        #region Addition

        public static Natural operator +(Natural x, Natural y)
        {
            Natural n = x.Clone();
            return Natural.Add(n, y);
        }

        #endregion

        #region Subtraction

        public static Natural operator -(Natural x, Natural y)
        {
            Natural n = x.Clone();
            return Natural.Subtract(n, y);
        }

        #endregion

        #region Multiplication

        public static Natural operator *(Natural x, Natural y)
        {
            Natural n = x.Clone();
            return Natural.Multiply(n, y);
        }

        #endregion

        #region Division

        public static Natural operator /(Natural x, Natural y)
        {
            Natural n = x.Clone();
            return Natural.Divide(n, y);
        }

        #endregion

        #region Modulus

        public static Natural operator %(Natural x, Natural y)
        {
            Natural r = x.Clone();
            return Natural.Modulus(r, y);
        }

        public static int operator %(Natural x, int y)
        {
            Natural q;
            int r;
            DivModSlow(x, y, out q, out r);
            return r;
        }

        public static long operator %(Natural x, long y)
        {
            Natural q;
            long r;
            DivModSlow(x, y, out q, out r);
            return r;
        }

        #endregion

        #region Functions

        public Natural Complement()
        {
            Natural n = this.Clone();
            return Natural.Complement(n);
        }

        public Natural Complement(int digits)
        {
            Natural n = this.Clone();
            return Natural.Complement(n, digits);
        }

        public static void DivMod(Natural n, Natural d, out Natural q, out Natural r)
        {
            DivModSlow(n, d, out q, out r);
        }
        public static void DivMod(Natural n, int d, out Natural q, out int r)
        {
            DivModSlow(n, d, out q, out r);
        }
        public static void DivMod(Natural n, long d, out Natural q, out long r)
        {
            DivModSlow(n, d, out q, out r);
        }

        /// <summary>
        /// Multiplication by repeated addition
        /// </summary>
        private static Natural MulSlow(Natural x, Natural y)
        {
            if (x == 0 || y == 0)
                return Natural.Zero;

            Natural n = Natural.Zero;
            n.PushLength(x.usedDigits * y.usedDigits);

            y = y.Clone();

            while (y > 0) // We already have 1x
            {
                Natural.Add(n, x);
                Natural.Decrement(y);
            }

            n.PopLength();

            return n;
        }

        private static Natural MulLong(Natural x, Natural y)
        {
            if (x == 0 || y == 0)
            {
                return Natural.Zero;
            }
            if (y == 1)
            {
                return x.Clone();
            }

            Natural total = Natural.Zero;

            for (int i = 0; i < x.usedDigits; i++)
            {
                Digit d = x.digits[i];
                Natural partial = MulDigit(y, d);
                Natural.ShiftLeft(partial, i);
                Natural.Add(total, partial);
            }

            return total;
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

        /*
        /// <summary>
        /// Implementation of Karatsuba's multiplication algorithm (>320 bits == 100 ints == 50 longs)
        /// </summary>
        private static Natural Mul2(Natural x, Natural y)
        {
            if (x.usedDigits == 1 || y.usedDigits == 1)
                return Mul1(x, y);

            // Calculates the size of the numbers
            long m = Math.Max(x.usedDigits, y.usedDigits);
            long m2 = (m + 1) / 2;

            // split the digit sequences about the middle 
            Natural high1, low1; Natural.SplitTo(x, m2, high1, low1);
            Natural high2, low2; Natural.SplitTo(y, m2, high2, low2);

            // 3 calls made to numbers approximately half the size
            Natural z0 = Mul2(low1, low2);
            Natural z1 = Mul2((low1 + high1), (low2 + high2));
            Natural z2 = Mul2(high1, high2);

            return z2 * Natural.Pow(Natural.Base, (2 * m2)) + (z1 - z2 - z0) * Natural.Pow(Natural.Base, (m2)) + z0;
        }
        */

        /// <summary>
        /// Division by repeated subtraction
        /// </summary>
        /// <param name="n">Dividend</param>
        /// <param name="d">Divisor</param>
        /// <param name="q">Quotient</param>
        /// <param name="r">Remainder</param>
        private static void DivModSlow(Natural n, Natural d, out Natural q, out Natural r)
        {
            if (d == 0)
                throw new DivideByZeroException();

            n = n.Clone();

            q = 0;

            while (n >= d)
            {
                Natural.Subtract(n, d);
                Natural.Increment(q);
            }

            r = n;
        }

        private static void DivModSlow(Natural n, int d, out Natural q, out int r)
        {
            if (d == 0)
                throw new DivideByZeroException();

            n = n.Clone();
            q = 0;

            //We only care about positive numbers
            uint _d = (uint)(d == int.MinValue ? d : Math.Abs(d));

            while (n >= _d)
            {
                Natural.Subtract(n, _d);
                Natural.Increment(q);
            }

            r = (int)n;
        }

        private static void DivModSlow(Natural n, long d, out Natural q, out long r)
        {
            if (d == 0)
                throw new DivideByZeroException();

            n = n.Clone();
            q = 0;

            //We only care about positive numbers
            ulong _d = (ulong)(d == long.MinValue ? d : Math.Abs(d));

            while (n >= _d)
            {
                Natural.Subtract(n, _d);
                Natural.Increment(q);
            }

            r = (long)n;
        }


        private static void DivModLong(Natural n, Natural d, out Natural q, out Natural r)
        {
            if (d == Natural.Zero)
                throw new DivideByZeroException();

            n = n.Clone();
            q = 0;

            while (n > d) // TODO Optimize this away
            {
                // Take enough digits to be larger than d
                IndexedArraySegment<Digit> seg = n.digits.GetSegment(0, d.usedDigits);
                for (int i = d.usedDigits - 1; d >= 0; d--)
                {
                    if (d.digits[i] > seg.Array[seg.Offset + i])
                    {
                        seg.Grow(seg.Count + 1);
                        break;
                    }
                }


            }

            r = n;
        }

        #endregion

        #endregion

        #region In-place arithmetic

        public static Natural Add(Natural x, Natural y)
        {
            int length = Math.Max(x.usedDigits, y.usedDigits);

            x.PushLength(length + 1);
            y.PushLength(length); // TODO Optimize this..

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

            return x;
        }

        public static Natural Subtract(Natural x, Natural y)
        {
            /*
             * If y is zero, return x
             * If x is larger than y, use Sub1
             * If x is smaller than y, would return negative
             * If x == y, set x to 0 and return x
             */

            if (y == 0)
                return x;
            else if (x > y)
            {
                return Natural.Complement(Natural.Add(Natural.Complement(x), y));
            }
            else if (x < y)
            {
                throw new InvalidOperationException(WouldBeNegative);
            }
            else
            {
                return x.Clear();
            }
        }

        public static Natural Multiply(Natural x, Natural y)
        {
            Natural res = MulLong(x, y);
            return x.SetValue(res);
        }

        public static Natural Divide(Natural x, Natural y)
        {
            if (y == Natural.Zero)
                throw new DivideByZeroException();

            Natural q, m;
            DivModSlow(x, y, out q, out m);
            x.Swap(q);
            return x;
        }

        public static Natural Modulus(Natural x, Natural y)
        {
            Natural q, m;
            DivModSlow(x, y, out q, out m);
            x.Swap(m);
            return x;
        }

        /// <summary>
        /// Returns the diminished radix complement of the number.
        /// </summary>
        /// <param name="n">A Natural number.</param>
        /// <returns></returns>
        public static Natural Complement(Natural n)
        {
            for (int i = 0; i < n.usedDigits; i++)
                n.digits[i] = Digit.MaxValue - n.digits[i];

            return n;
        }

        /// <summary>
        /// Returns the diminished radix complement of the number.
        /// </summary>
        /// <param name="n">A Natural number.</param>
        /// <param name="digits">The significant amount of digits to be modified.</param>
        /// <returns></returns>
        public static Natural Complement(Natural n, int digits)
        {
            n.PushLength(digits);

            for (int i = 0; i < digits; i++)
                n.digits[i] = Digit.MaxValue - n.digits[i];

            n.PopLength();
            return n;
        }

        public static Natural Increment(Natural n)
        {
            return Natural.Add(n, 1);
        }

        public static Natural Decrement(Natural n)
        {
            return Natural.Subtract(n, 1);
        }

        public static Natural ShiftLeft(Natural n, int numDigits)
        {
            if (numDigits == 0)
                return n;

            if (numDigits < 0)
                return Natural.ShiftRight(n, -numDigits);

            n.digits.Grow(n.digits.Count + numDigits);
            n.digits.ShiftRight(numDigits);
            n.usedDigits += numDigits;
            return n;
        }

        public static Natural ShiftRight(Natural n, int numDigits)
        {
            if (numDigits == 0)
                return n;

            if (numDigits < 0)
                return Natural.ShiftLeft(n, -numDigits);

            n.digits.ShiftLeft(numDigits); // Actual array shift direction is opposite of digit order
            if (numDigits >= n.usedDigits)
                n.usedDigits = 1;
            else
                n.usedDigits -= numDigits;

            return n;
        }

        #endregion

        #region Natural.Math

        public static Natural Pow(Natural num, Natural exponent)
        {
            if (exponent == 0)
                return Natural.One;

            if (exponent == 1)
                return num.Clone();

            if (num == 0)
                return Natural.Zero;

            Natural result = Natural.One;

            Natural q = exponent;
            int r;
            while (q > 1)
            {
                Natural.DivMod(q, 2, out q, out r);

                if (r == 1)
                    Natural.Multiply(result, num);

                Natural.Multiply(num, num);

            }

            return result * num;
        }

        #endregion

        #region Comparison

        #region Equality

        public static bool operator ==(Natural x, Natural y)
        {
            if (x.usedDigits != y.usedDigits)
                return false;
            else
            {
                for (int d = 0; d < x.usedDigits; d++)
                {
                    if (x.digits[d] != y.digits[d])
                        return false; //Inequal digit
                }
            }

            return true; //Numbers were equal
        }
        public static bool operator ==(Natural x, int y)
        {
            if (y < 0)
                return false;

            if (x.usedDigits == 1 && x.digits[0] == (uint)y)
                return true;

            return false;
        }
        public static bool operator ==(Natural x, long y)
        {
            if (y < 0)
                return false;
            if (x.usedDigits > 2)
                return false;

            return x == new Natural(y);
        }

        #endregion

        #region Inequality

        public static bool operator !=(Natural x, Natural y)
        {
            if (x.usedDigits != y.usedDigits)
                return true;
            else
            {
                for (int d = 0; d < x.usedDigits; d++)
                {
                    if (x.digits[d] != y.digits[d])
                        return true; //Inequal digit
                }
            }

            return false; //Numbers were equal
        }
        public static bool operator !=(Natural x, int y)
        {
            if (x.usedDigits > 1)
                return true;

            return x.digits[0] != y;
        }
        public static bool operator !=(Natural x, long y)
        {
            if (y < 0)
                return true;
            if (x.usedDigits > 2)
                return true;

            return x != new Natural(y);
        }

        #endregion

        #region Less than

        public static bool operator <(Natural x, Natural y)
        {
            if (x.usedDigits < y.usedDigits)
                return true;
            else if (x.usedDigits > y.usedDigits)
                return false;
            else
            {
                for (int d = x.usedDigits - 1; d >= 0; d--)
                {
                    if (x.digits[d] < y.digits[d])
                        return true;
                    if (x.digits[d] > y.digits[d])
                        return false;
                }
            }

            return false; //Numbers were equal
        }
        public static bool operator <(Natural x, int y)
        {
            if (y < 0)
                return false;
            if (x.usedDigits > 1)
                return false;

            return x.digits[0] < y;
        }
        public static bool operator <(Natural x, long y)
        {
            if (y < 0)
                return false;
            if (x.usedDigits > 2)
                return false;

            return x < new Natural(y);
        }

        public static bool operator <=(Natural x, Natural y)
        {
            if (x.usedDigits < y.usedDigits)
                return true;
            else if (x.usedDigits > y.usedDigits)
                return false;
            else
            {
                for (int d = x.usedDigits - 1; d >= 0; d--)
                {
                    if (x.digits[d] < y.digits[d])
                        return true;
                    if (x.digits[d] > y.digits[d])
                        return false;
                }
            }

            return true; //Numbers were equal
        }
        public static bool operator <=(Natural x, int y)
        {
            if (y < 0)
                return false;
            if (x.usedDigits > 1)
                return false;

            return x.digits[0] <= y;
        }
        public static bool operator <=(Natural x, long y)
        {
            if (y < 0)
                return false;
            if (x.usedDigits > 2)
                return false;

            return x <= new Natural(y);
        }

        #endregion

        #region Greater than

        public static bool operator >(Natural x, Natural y)
        {
            if (x.usedDigits > y.usedDigits)
                return true;
            else if (x.usedDigits < y.usedDigits)
                return false;
            else
            {
                for (int d = x.usedDigits - 1; d >= 0; d--)
                {
                    if (x.digits[d] > y.digits[d])
                        return true;
                    if (x.digits[d] < y.digits[d])
                        return false;
                }
            }

            return false;
        }
        public static bool operator >(Natural x, int y)
        {
            if (y < 0)
                return true;
            if (x.usedDigits > 1)
                return true;

            return x.digits[0] > y;
        }
        public static bool operator >(Natural x, long y)
        {
            if (y < 0)
                return true;
            if (x.usedDigits > 2)
                return true;

            return x > new Natural(y);
        }

        public static bool operator >=(Natural x, Natural y)
        {
            if (x.usedDigits > y.usedDigits)
                return true;
            else if (x.usedDigits < y.usedDigits)
                return false;
            else
            {
                for (int d = x.usedDigits - 1; d >= 0; d--)
                {
                    if (x.digits[d] > y.digits[d])
                        return true;
                    if (x.digits[d] < y.digits[d])
                        return false;
                }
            }

            return true; //Numbers were equal
        }
        public static bool operator >=(Natural x, int y)
        {
            if (y < 0)
                return true;
            if (x.usedDigits > 1)
                return true;

            return x.digits[0] >= y;
        }
        public static bool operator >=(Natural x, long y)
        {
            if (y < 0)
                return true;
            if (x.usedDigits > 2)
                return true;

            return x >= new Natural(y);
        }

        #endregion

        #endregion

        #region Implicit casting

        public static implicit operator Natural(byte x)
        {
            return new Natural((uint)x);
        }

        public static implicit operator Natural(short x)
        {
            return new Natural((int)x);
        }

        public static implicit operator Natural(int x)
        {
            return new Natural(x);
        }

        [CLSCompliant(false)]
        public static implicit operator Natural(uint x)
        {
            return new Natural(x);
        }

        public static implicit operator Natural(long x)
        {
            return new Natural(x);
        }

        [CLSCompliant(false)]
        public static implicit operator Natural(ulong x)
        {
            return new Natural(x);
        }

        #endregion

        #region Explicit casting

        public static explicit operator byte (Natural x)
        {
            return (byte)(x.digits[0] & 0xFF);
        }

        public static explicit operator short (Natural x)
        {
            return (short)(x.digits[0] & 0xFFFF);
        }

        public static explicit operator int (Natural x)
        {
            return (int)(x.digits[0] & 0x7FFFFFFF);
        }

        public static explicit operator long (Natural x)
        {
            if (x.usedDigits > 1)
                return ((long)(x.digits[1] & 0x7FFFFFFF) << 32) | x.digits[0];
            else
                return x.digits[0];
        }

        [CLSCompliant(false)]
        public static explicit operator ulong (Natural x)
        {
            if (x.usedDigits > 1)
                return ((ulong)x.digits[1] << 32) | x.digits[0];
            else
                return x.digits[0];
        }

        public static explicit operator BigInteger(Natural x)
        {
            byte[] lEndianArray = x.GetLittleEndianByteArray();
            Array.Resize(ref lEndianArray, lEndianArray.Length + 1);
            lEndianArray[lEndianArray.Length - 1] = 0;

            return new BigInteger(lEndianArray);
        }

        public static explicit operator Natural(BigInteger x)
        {
            if (x.Sign < 0)
                throw new InvalidOperationException(WouldBeNegative);

            byte[] lEndianArray = x.ToByteArray();
            if (lEndianArray.Length == 0)
                return Natural.Zero;

            uint[] converted = Helper.ConvertByteArrayToUIntArrayLE(lEndianArray);

            return new Natural(converted);
        }

        #endregion

        #region Overloading

        public override bool Equals(object obj)
        {
            if (obj is Natural)
            {
                return this == (Natural)obj;
            }
            return false;
        }

        public override int GetHashCode()
        {
            uint hash = 0;
            unchecked
            {
                for (int i = 0; i < usedDigits; i++)
                {
                    hash ^= digits[i] * hash + digits[i];
                }
            }
            return (int)hash;
        }

        public override string ToString()
        {
            return ToString(10);
        }

        public string ToString(int radix, string chars = "0123456789ABCDEFHIJKLMNOPQRSTUVWXYZ")
        {
            if (radix == -1 || radix == 0 || radix == 1)
                throw new ArgumentException("Radix cannot be -1, 0 or 1.");

            char[] baseChars = chars.ToCharArray();

            StringBuilder sb = new StringBuilder();
            Natural value = this.Clone();
            Natural q;
            int r;

            do
            {
                Natural.DivModSlow(value, radix, out q, out r);
                sb.Insert(0, baseChars[r]);
                value = q;
            } while (value > 0);

            return sb.ToString();
        }

        #endregion

        #region Supplementary

        public Natural Clone()
        {
            Digit[] dgt = digits.ToArray();
            Natural i = new Natural(dgt, usedDigits);

            i.lengths = new Stack<int>(this.lengths);

            return i;
        }

        /// <summary>
        /// (Re)initializes the value to Natural.Zero.
        /// </summary>
        /// <returns></returns>
        public Natural Clear()
        {
            usedDigits = 0;
            digits = new Digit[default_number_size];
            lengths.Clear();

            return this;
        }

        /// <summary>
        /// Places the value of newVal in this instance.
        /// </summary>
        /// <param name="newVal"></param>
        /// <returns></returns>
        public Natural SetValue(Natural newVal)
        {
            digits = newVal.digits.ToArray();
            lengths = new Stack<int>(newVal.lengths);
            usedDigits = newVal.usedDigits;

            return this;
        }

        /// <summary>
        /// Swaps newVal into this instance, and places the old value in newVal.
        /// </summary>
        /// <param name="newVal"></param>
        /// <returns></returns>
        public Natural Swap(Natural newVal)
        {
            IndexedArraySegment<Digit> newdigits = newVal.digits;
            Stack<int> lll = newVal.lengths;
            int usdgt = newVal.usedDigits;

            newVal.digits = digits;
            newVal.lengths = lengths;
            newVal.usedDigits = usedDigits;

            digits = newdigits;
            lengths = lll;
            usedDigits = usdgt;

            return this;
        }

        /// <summary>
        /// Called after every operation to ensure accurate digit count. User should also call after direct access to digit array.
        /// </summary>
        public void NotifyUpdate()
        {
            // Loop from end of array to start to find the index of the first significant digit
            usedDigits = digits.Count;
            while (usedDigits > 1 && digits[usedDigits - 1] == 0)
                usedDigits--;

            //Debug.WriteLine("Counted {0} digits", usedDigits);
            //Grow the array to make room for more digits
            Grow(usedDigits);
        }

        public byte[] GetLittleEndianByteArray()
        {
            return Helper.ConvertUIntArrayToByteArrayLE(GetLittleEndianUIntArray());
        }

        [CLSCompliant(false)]
        public uint[] GetLittleEndianUIntArray()
        {
            uint[] d = digits.ToArray();
            Array.Resize(ref d, usedDigits);
            return d;
        }

        private void PushLength(int length)
        {
            lengths.Push(length);

            Grow(length);
        }

        private void PopLength()
        {
            int min = 0;
            if (lengths.Count > 0)
                min = lengths.Pop();

            GrowShrink(min);
        }

        private void Grow(int length)
        {
            int tl = RoundToPow2(length);
            if (digits.Count >= tl)
                return;

            //Debug.WriteLine("Grow array from {0} to {1} digits", digits.Count, tl);
            digits.Grow(tl);
        }

        private void GrowShrink(int length)
        {
            int tl = RoundToPow2(length);

            if (digits.Count == tl)
                return;

            if (digits.Count > tl) // We want to shrink it
            {
                int minl = RoundToPow2(usedDigits); // Minimum length to still keep digits
                if (tl < minl)
                    return; // Can't shrink more

                //Debug.WriteLine("Shrink array from {0} to {1} digits", digits.Count, tl);
                digits.Shrink(minl);
            }
            else // Want to grow
            {
                //Debug.WriteLine("Grow array from {0} to {1} digits", digits.Count, tl);
                digits.Grow(tl);
            }
        }


        // Thanks to Larry Gritz - http://stackoverflow.com/questions/364985/algorithm-for-finding-the-smallest-power-of-two-thats-greater-or-equal-to-a-giv
        private static int RoundToPow2(int x)
        {
            if (x < 0)
                return 0;
            --x;
            x |= x >> 1;
            x |= x >> 2;
            x |= x >> 4;
            x |= x >> 8;
            x |= x >> 16;
            return x + 1;
        }

        #endregion

        #region Interfaces

        public string ToString(string format, IFormatProvider formatProvider)
        {
            if (string.IsNullOrWhiteSpace(format) || format == "G")
            {
                return ToString();
            }
            else
            {
                int radix = 0;
                if (int.TryParse(format, out radix))
                {
                    return ToString(radix);
                }
            }

            //Treat anything we don't support as general base 10
            return ToString();
        }

        public TypeCode GetTypeCode()
        {
            return TypeCode.Object;
        }

        public bool ToBoolean(IFormatProvider provider)
        {
            return this != 0;
        }

        public char ToChar(IFormatProvider provider)
        {
            return (char)(digits[0] & 0x000000FF);
        }

        [CLSCompliant(false)]
        public sbyte ToSByte(IFormatProvider provider)
        {
            return (sbyte)(digits[0] & 0x0000007F);
        }

        public byte ToByte(IFormatProvider provider)
        {
            return (byte)(digits[0] & 0x000000FF);
        }

        public short ToInt16(IFormatProvider provider)
        {
            return (Int16)(digits[0] & 0x00007FFF);
        }

        [CLSCompliant(false)]
        public ushort ToUInt16(IFormatProvider provider)
        {
            return (UInt16)(digits[0] & 0x00008FFF);
        }

        public int ToInt32(IFormatProvider provider)
        {
            return (Int32)(digits[0] & 0x7FFFFFFFF);
        }

        [CLSCompliant(false)]
        public uint ToUInt32(IFormatProvider provider)
        {
            return digits[0];
        }

        public long ToInt64(IFormatProvider provider)
        {
            return ((Int64)(digits[1] & 0x7FFFFFFF)) << 32 | digits[0];
        }

        [CLSCompliant(false)]
        public ulong ToUInt64(IFormatProvider provider)
        {
            return ((UInt64)digits[1]) << 32 | digits[0];
        }

        public float ToSingle(IFormatProvider provider)
        {
            return ToUInt32(provider);
        }

        public double ToDouble(IFormatProvider provider)
        {
            return ToUInt64(provider);
        }

        public decimal ToDecimal(IFormatProvider provider)
        {
            return ToUInt64(provider);
        }

        public DateTime ToDateTime(IFormatProvider provider)
        {
            return new DateTime(ToInt64(provider));
        }

        public string ToString(IFormatProvider provider)
        {
            return ToString("G", provider);
        }

        public object ToType(Type conversionType, IFormatProvider provider)
        {
            throw new NotImplementedException();
        }

        public int CompareTo(Natural other)
        {
            if (other.usedDigits < this.usedDigits) //The other was smaller, I'm last
                return 1;
            else if (other.usedDigits > this.usedDigits) //The other was larger, I'm first
                return -1;
            else
            {
                for (int d = other.usedDigits; d >= 0; d--)
                {
                    int res = other.digits[d].CompareTo(this.digits[d]);
                    if (res != 0)
                        return res;
                }
            }

            return 0; //Numbers were equal
        }

        public int CompareTo(object obj)
        {
            if (obj is Natural)
            {
                return CompareTo(obj as Natural);
            }
            else
            {
                return -1;
            }
        }

        public bool Equals(Natural other)
        {
            return this == other;
        }


        #endregion

    }
}
