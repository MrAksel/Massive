using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

using Carry = System.UInt64;
using Digit = System.UInt32;


namespace Massive.Mathematics
{
    public class Natural : IComparable, IFormattable, IConvertible, IComparable<Natural>, IEquatable<Natural>
    {
        private const string WouldBeNegative = "Operation would return a negative value.";

        /// <summary>
        /// Multiplier (sub_array_size) for underlying VirtualArray. 
        /// </summary>
        public static int default_array_size = 16384;

        private long minDigits;
        private long usedDigits;
        private Stack<long> lengths;
        private VirtualArray<Digit> digits;

        public static Natural Zero
        {
            get
            {
                return new Natural();
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
            if (default_array_size < 1)
                throw new ArgumentOutOfRangeException("default_array_size has to be larger than 0");
            digits = new VirtualArray<Digit>(default_array_size);

            minDigits = 0;
            usedDigits = 0;
            lengths = new Stack<long>();
        }

        public Natural(int value)
            : this()
        {
            digits[0] = (uint)Math.Abs((long)value);
            usedDigits = (value > 0 ? 1 : 0);
        }

        [CLSCompliant(false)]
        public Natural(uint value)
            : this()
        {
            digits[0] = value;
            usedDigits = (value > 0 ? 1 : 0);
        }

        public Natural(long value)
            : this()
        {
            ulong v = (value == long.MinValue ? (ulong)value : (ulong)Math.Abs(value));

            digits[0] = (uint)(v & 0x00000000FFFFFFFF);
            digits[1] = (uint)((v & 0xFFFFFFFF00000000) >> 32);
            usedDigits = (digits[1] > 0 ? 2 : (digits[0] > 0 ? 1 : 0));
        }

        [CLSCompliant(false)]
        public Natural(ulong value)
            : this()
        {
            digits[0] = (uint)(value & 0x00000000FFFFFFFF);
            digits[1] = (uint)((value & 0xFFFFFFFF00000000) >> 32);
            usedDigits = (digits[1] > 0 ? 2 : (digits[0] > 0 ? 1 : 0));
        }

        [CLSCompliant(false)]
        public Natural(VirtualArray<Digit> digits)
        {
            this.digits = digits;

            NotifyUpdate();
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

        public Natural Complement(long digits)
        {
            Natural n = this.Clone();
            return Natural.Complement(n, digits);
        }


        /// <summary>
        /// Multiplication by repeated addition
        /// </summary>
        private static Natural Mul1(Natural x, Natural y)
        {
            Natural n = new Natural();
            n.PushLength(x.usedDigits * y.usedDigits);

            y = y.Clone();

            while (y > 0)
            {
                Natural.Add(n, x);
                Natural.Decrement(y);
            }

            n.PopLength();

            return n;
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

        /*
        private static void DivModLong(Natural dividend, Natural divisor, out Natural quotient, out Natural remainder)
        {
            quotient = 0; 
            r = 0;


            //
            //    o = offset from largest significant number (number of digits skipped already)
            //    n = digits in divisor
            //    Loop
            //        Append digits from dividend to r (starting at o) until number of digits == n. o += n
            //        Compare r to divisor, if less, include one more digit in r. o++
            //        Divide r by divisor, add the one-digit quotient to the final quotient. Save the remainder in r
             


            VirtualArray<Digit> tmpQuotient = new VirtualArray<Digit>(default_array_size);
            long digitCount = 0;

            //Number of digits in divisor
            long n = divisor.usedDigits;
            //Number of digits skipped in dividend
            long o = 0;

            VirtualArray<Digit> cDiv = dividend.digits.Clone();
            VirtualArrayFragment<Digit> tmpDividend = new VirtualArrayFragment<Digit>(cDiv, 0, 0);
            Natural r = new Natural(tmpDividend);

            Natural tmpq;

            while (dividend.usedDigits - o > 0) //While we have digits left to take
            {
                long mxDigit = dividend.usedDigits - o;
                tmpDividend.SetLength(n); //tmpDividend needs at least n digits to be larger than divisor
                r.NotifyUpdate();

                if(r < divisor) // We have to add one more digit
                {
                    //TODO also add a zero to quotient
                    if (n + 1 > mxDigit) //Check if possible to add another
                    {
                        //Nope, we are out of digits. 
                        //TODO done, set remainder and quotient
                    }
                    else
                    {
                        //We add a new digit by expanding the array
                        tmpDividend.MoveLength(+1);
                        tmpDividend.MoveOffset(-1);
                        r.NotifyUpdate();
                    }
                }

                //Divide the selected digits of the dividend by the divisor, and place the remainder in r
                DivModSlow(r, divisor, out tmpq, out r);

                if (tmpq.usedDigits > 1)
                    throw new Exception();
            }
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

        #endregion

        #endregion

        #region In-place arithmetic

        public static Natural Add(Natural x, Natural y)
        {
            long length = Math.Max(x.usedDigits, y.usedDigits);

            x.PushLength(length + 1);
            y.PushLength(length);

            Digit carry = 0;
            for (long i = 0; i < length; i++)
            {
                Carry res = x.digits[i] + y.digits[i] + carry;
                Digit val = (Digit)res;
                carry = (val < res ? (Digit)1 : (Digit)0);

                x.digits[i] = val;
            }
            x.digits[length] = carry;
            x.usedDigits = length + carry; // If carry == 1 we used another extra digit

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
            if (y == Natural.Zero)
                return Natural.Zero;

            Natural n = x.Clone();
            y = y - 1; // We already have 1x

            while (y > 0)
            {
                Natural.Add(x, n);
                Natural.Decrement(y);
            }
            return x;
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
        public static Natural Complement(Natural n, long digits)
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

        #endregion

        #region Comparison

        #region Equality

        public static bool operator ==(Natural x, Natural y)
        {
            if (x.usedDigits != y.usedDigits)
                return false;
            else
            {
                for (long d = 0; d < x.usedDigits; d++)
                {
                    if (x.digits[d] != y.digits[d])
                        return false; //Inequal digit
                }
            }

            return true; //Numbers were equal
        }
        public static bool operator ==(Natural x, int y)
        {
            if (x.usedDigits == 1 && x.digits[0] == y)
                return true;
            else if (x.usedDigits == 0 && y == 0)
                return true;
            return false;
        }
        public static bool operator ==(Natural x, long y)
        {
            if (y < 0)
                return false;
            return x == new Natural(y);
        }

        #endregion

        #region Inequality

        public static bool operator !=(Natural x, Natural y)
        {
            if (x.usedDigits != y.usedDigits)
                return false;
            else
            {
                for (long d = 0; d < x.usedDigits; d++)
                {
                    if (x.digits[d] != y.digits[d])
                        return true; //Inequal digit
                }
            }

            return false; //Numbers were equal
        }
        public static bool operator !=(Natural x, int y)
        {
            return x.digits[0] != y;
        }
        public static bool operator !=(Natural x, long y)
        {
            if (y < 0)
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
                for (long d = x.usedDigits; d >= 0; d--)
                {
                    int res = x.digits[d].CompareTo(y.digits[d]);
                    switch (res)
                    {
                        case 1:
                            return false; //Digit in x was larger than digit in y
                        case -1:
                            return true; //Digit in x was smaller
                    }  //Default - equal digits
                }
            }

            return false; //Numbers were equal
        }
        public static bool operator <(Natural x, int y)
        {
            if (y < 0)
                return false;
            return x.digits[0] < y;
        }
        public static bool operator <(Natural x, long y)
        {
            if (y < 0)
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
                for (long d = x.usedDigits; d >= 0; d--)
                {
                    int res = x.digits[d].CompareTo(y.digits[d]);
                    switch (res)
                    {
                        case 1:
                            return false; //Digit in x was larger than digit in y
                        case -1:
                            return true; //Digit in x was smaller
                    }  //Default - equal digits
                }
            }

            return true; //Numbers were equal
        }
        public static bool operator <=(Natural x, int y)
        {
            if (y < 0)
                return false;
            return x.digits[0] <= y;
        }
        public static bool operator <=(Natural x, long y)
        {
            if (y < 0)
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
                for (long d = x.usedDigits; d >= 0; d--)
                {
                    int res = x.digits[d].CompareTo(y.digits[d]);
                    switch (res)
                    {
                        case 1:
                            return true;
                        case -1:
                            return false;
                    }
                }
            }

            return false;
        }
        public static bool operator >(Natural x, int y)
        {
            if (y < 0)
                return true;
            return x.digits[0] > y;
        }
        public static bool operator >(Natural x, long y)
        {
            if (y < 0)
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
                for (long d = x.usedDigits; d >= 0; d--)
                {
                    int res = x.digits[d].CompareTo(y.digits[d]);
                    switch (res)
                    {
                        case 1:
                            return true;
                        case -1:
                            return false;
                    }
                }
            }

            return true; //Numbers were equal
        }
        public static bool operator >=(Natural x, int y)
        {
            if (y < 0)
                return true;
            return x.digits[0] >= y;
        }
        public static bool operator >=(Natural x, long y)
        {
            if (y < 0)
                return true;
            return x >= new Natural(y);
        }

        #endregion

        #endregion

        #region Implicit casting

        public static implicit operator Natural(byte x)
        {
            return new Natural((int)x);
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
            throw new NotImplementedException();
        }

        public static explicit operator short (Natural x)
        {
            throw new NotImplementedException();
        }

        public static explicit operator int (Natural x)
        {
            return (int)(x.digits[0] & 0x7FFFFFFF);
        }

        public static explicit operator long (Natural x)
        {
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
            byte[] lEndianArray = x.ToByteArray();
            if (lEndianArray.Length < 2)
                return new Natural();

            Array.Resize(ref lEndianArray, lEndianArray.Length - 1);
            uint[] converted = Helper.ConvertByteArrayToUIntArray(lEndianArray);

            return new Natural(new VirtualArray<uint>(converted, default_array_size));
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
            int hash = 0;
            unchecked
            {
                for (int i = 0; i < usedDigits; i++)
                {
                    hash ^= (int)digits[i] * hash + (int)digits[i];
                }
            }
            return hash;
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
            VirtualArray<Digit> dgt = digits.Clone();
            Natural i = new Natural(dgt);

            i.lengths = new Stack<long>(this.lengths);
            i.minDigits = this.minDigits;

            return i;
        }

        /// <summary>
        /// (Re)initializes the value to Natural.Zero.
        /// </summary>
        /// <returns></returns>
        public Natural Clear()
        {
            minDigits = 0;
            usedDigits = 0;
            digits.Clear();
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
            digits = newVal.digits.Clone();
            lengths = newVal.lengths;
            minDigits = newVal.minDigits;
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
            VirtualArray<Digit> newdigits = newVal.digits;
            Stack<long> lll = newVal.lengths;
            long minlll = newVal.minDigits;
            long usdgt = newVal.usedDigits;

            newVal.digits = digits;
            newVal.lengths = lengths;
            newVal.minDigits = minDigits;
            newVal.usedDigits = usedDigits;

            digits = newdigits;
            lengths = lll;
            minDigits = minlll;
            usedDigits = usdgt;

            return this;
        }

        /// <summary>
        /// Called after every operation to ensure accurate digit count. User should also call after direct access to digit array.
        /// </summary>
        public void NotifyUpdate()
        {
            // Loop from end of array to start to find the index of the first significant digit
            usedDigits = digits.Length;
            while (usedDigits > 0 && digits[usedDigits - 1] == 0)
                usedDigits--;

            //Grow the array to make room for more digits
            digits.Resize(Math.Max(usedDigits + 1, minDigits));
        }

        public byte[] GetLittleEndianByteArray()
        {
            return Helper.ConvertUIntArrayToByteArray(GetLittleEndianUIntArray());
        }

        [CLSCompliant(false)]
        public uint[] GetLittleEndianUIntArray()
        {
            return digits.ToArray();
        }

        private void PushLength(long length)
        {
            lengths.Push(length);
            minDigits = length;
            digits.Grow(minDigits);
        }

        private void PopLength()
        {
            if (lengths.Count > 0)
                minDigits = lengths.Pop();
            else
                minDigits = 0;
            digits.Grow(minDigits);
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
                for (long d = other.usedDigits; d >= 0; d--)
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
