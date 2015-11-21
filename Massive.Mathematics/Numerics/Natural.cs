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
using Massive.Mathematics.Numerics.Adders;
using Massive.Mathematics.Numerics.Subtracters;
using Massive.Mathematics.Numerics.Multipliers;
using Massive.Mathematics.Numerics.Dividers;
using Massive.Mathematics.Extensions;

namespace Massive.Mathematics.Numerics
{
    public class Natural : IComparable, IFormattable, IConvertible, IComparable<Natural>, IEquatable<Natural>
    {
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

        internal int usedDigits;
        internal Stack<int> lengths;

        internal IndexedArraySegment<Digit> digits;

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

        #region Operation managers

        static IAdderManager adderManager = new DefaultAdderManager();
        static ISubtracterManager subtracterManager = new DefaultSubtracterManager();
        static IMultiplierManager multiplierManager = new DefaultMultiplierManager();
        static IDividerManager dividerManager = new DefaultDividerManager();

        public static IAdderManager AdderManager
        {
            get
            {
                return adderManager;
            }
            set
            {
                if (value == null)
                    throw new ArgumentNullException();
                adderManager = value;
            }
        }

        public static ISubtracterManager SubtracterManager
        {
            get
            {
                return subtracterManager;
            }
            set
            {
                if (value == null)
                    throw new ArgumentNullException();
                subtracterManager = value;
            }
        }

        public static IMultiplierManager MultiplierManager
        {
            get
            {
                return multiplierManager;
            }
            set
            {
                if (value == null)
                    throw new ArgumentNullException();
                multiplierManager = value;
            }
        }

        public static IDividerManager DividerManager
        {
            get
            {
                return dividerManager;
            }
            set
            {
                if (value == null)
                    throw new ArgumentNullException();
                dividerManager = value;
            }
        }

        #endregion

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
                throw new InvalidOperationException(Resources.Strings.WouldBeNegative);

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
                throw new InvalidOperationException(Resources.Strings.WouldBeNegative);

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

            digits[0] = (uint)(value & 0x00000000FFFFFFFF);
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

        [CLSCompliant(false)]
        public Natural(IndexedArraySegment<Digit> digits, int nDigits = -1)
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
                throw new InvalidOperationException(Resources.Strings.WouldBeNegative);
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

        // TODO Modulo for ints and longs

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

        public static void DivRem(Natural n, Natural d, out Natural q, out Natural r)
        {
            DividerManager.GetDivider(n, d).DivRem(n, d, out q, out r);
        }

        // TODO DivRem for int and long types

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

        #endregion

        #endregion

        #region In-place arithmetic

        public static Natural Add(Natural x, Natural y)
        {
            adderManager.GetAdder(x, y).Add(x, y);
            return x;
        }

        public static Natural Subtract(Natural x, Natural y)
        {
            subtracterManager.GetSubtracter(x, y).Subtract(x, y);
            return x;
        }

        public static Natural Multiply(Natural x, Natural y)
        {
            multiplierManager.GetMultiplier(x, y).Multiply(x, y);
            return x;
        }

        public static Natural Divide(Natural x, Natural y)
        {
            if (y == Natural.Zero)
                throw new DivideByZeroException();

            dividerManager.GetDivider(x, y).Divide(x, y);
            return x;
        }

        public static Natural Modulus(Natural x, Natural y)
        {
            if (y == Natural.Zero)
                throw new DivideByZeroException();

            dividerManager.GetDivider(x, y).Remainder(x, y);
            return x;
        }

        /// <summary>
        /// Returns the diminished radix complement of the number.
        /// </summary>
        /// <param name="n">A Natural number.</param>
        /// <returns></returns>
        public static Natural Complement(Natural n)
        {
            int highestDigit = 0; // Position of last nonzero digit
            for (int i = 0; i < n.usedDigits; i++)
            {
                n.digits[i] = Digit.MaxValue - n.digits[i];
                if (n.digits[i] != 0)
                    highestDigit = i;
            }

            highestDigit = highestDigit + 1;
            n.usedDigits = highestDigit; // We checked all digits - no nonzero above highestDigit

            return n;
        }

        public static Natural Complement(Natural n, int digits)
        {
            n.PushLength(digits);

            int highestDigit = 0; // Position of last nonzero digit
            for (int i = 0; i < digits; i++)
            {
                n.digits[i] = Digit.MaxValue - n.digits[i];
                if (n.digits[i] != 0)
                    highestDigit = i;
            }

            highestDigit = highestDigit + 1;
            if (digits == n.usedDigits)
                n.usedDigits = highestDigit; // We complemented every digit, no nonzero digits after highestDigit
            else if (n.usedDigits < highestDigit)
                n.usedDigits = highestDigit; // All digits above usedDigits became Digit.MaxValue
            else
                ;   // We didn't check up to usedDigits - so there are still nonzero digits above highestDigit

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

            n.digits.ExpandWindow(n.digits.Count + numDigits);
            n.digits.ShiftContentsRight(numDigits);
            n.usedDigits += numDigits;
            return n;
        }

        public static Natural ShiftRight(Natural n, int numDigits)
        {
            if (numDigits == 0)
                return n;

            if (numDigits < 0)
                return Natural.ShiftLeft(n, -numDigits);

            n.digits.ShiftContentsLeft(numDigits); // Actual array shift direction is opposite of digit order
            if (numDigits >= n.usedDigits)
                n.usedDigits = 1;
            else
                n.usedDigits -= numDigits;

            return n;
        }

        public static Natural BitShiftLeft(Natural n, long numBits)
        {
            if (numBits == 0)
                return n;

            if (numBits < 0)
                return Natural.BitShiftRight(n, -numBits);

            int extrabits = UnsignedMath.HighestBitSetPosition(n.digits[n.usedDigits - 1]); // Additional bits used in the most significant digit
            int extradigits = (int)((extrabits + numBits) / 32);
            int totaldigits = n.usedDigits + extradigits;

            n.PushLength(totaldigits);              // Might shift past window of digit buffer, expand up front in case this happens
            n.digits.BitShiftContentRight(numBits); // Actual array shift direction is opposite of digit order
            n.PopLength();

            n.usedDigits = totaldigits;  // Increase usedDigits by new digits after shifting

            return n;
        }

        public static Natural BitShiftRight(Natural n, long numBits)
        {
            if (numBits == 0)
                return n;

            if (numBits < 0)
                return Natural.BitShiftLeft(n, -numBits);

            int extrabits = UnsignedMath.HighestBitSetPosition(n.digits[n.usedDigits - 1]) + 1; // If we shift 'extrabits' times, the most significant digit becomes zero
            int totbits = (n.usedDigits - 1) * 32 + extrabits;  // Have to shift 'totbits' times to make whole number zero

            n.digits.BitShiftContentLeft(numBits); // Actual array shift direction is opposite of digit order
            n.usedDigits = (int)(totbits - numBits + 31) / 32; // Round division up
            if (n.usedDigits < 1)
                n.usedDigits = 1;

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

            Natural sqr = num.Clone();
            Natural q = exponent;
            Natural r;
            while (q > 1)
            {
                Natural.DivRem(q, 2, out q, out r);

                if (r == 1)
                    Natural.Multiply(result, num);

                Natural.Multiply(sqr, sqr);

            }

            return Natural.Multiply(sqr, result);
        }

        public static Natural Log2(Natural num)
        {
            return num.usedDigits * 32;
        }

        public static Natural Log32(Natural num)
        {
            return num.usedDigits;
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
                throw new InvalidOperationException(Resources.Strings.WouldBeNegative);

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
            Natural r;

            do
            {
                Natural.DivRem(value, radix, out q, out r);
                int ri = (int)r;
                sb.Insert(0, baseChars[ri]);
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

            if (usedDigits == 0)
                usedDigits = 1;

            //Debug.WriteLine("Counted {0} digits", usedDigits);
            //Grow the array to make room for more digits
            Grow(usedDigits);
        }

        public void NotifyUpdate(int startPos)
        {
            // Loop from end of array to start to find the index of the first significant digit
            usedDigits = startPos;
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

        public void PushLength(int length)
        {
            lengths.Push(length);

            Grow(length);
        }

        public void PopLength()
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
            digits.ExpandWindow(tl);
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
                digits.ShrinkWindow(minl);
            }
            else // Want to grow
            {
                //Debug.WriteLine("Grow array from {0} to {1} digits", digits.Count, tl);
                digits.ExpandWindow(tl);
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
            if (this.usedDigits < other.usedDigits) // This was smaller ( CompareTo() < 0 )
                return -1;
            else if (this.usedDigits > other.usedDigits) // This was larger ( CompareTo() > 0 )
                return 1;
            else
            {
                for (int d = other.usedDigits - 1; d >= 0; d--)
                {
                    int res = digits[d].CompareTo(other.digits[d]);
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
