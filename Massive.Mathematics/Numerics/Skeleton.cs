using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Carry = System.UInt64;
using Digit = System.UInt32;

/*
 * Expression ( e^2z + sin(x) )                 // jupp
 *   - Complex ( 5 + 2i )                       // *yes*
 *      - Real expression ( sin(4) + 2 )        // nope
 *        - Algebraic ( 5x^2 - 2x + 3 = 0 )     // not needed
 *          - Rational ( x = 5/2)               // needed
 *            - Integer ( x = 1 )               // needed
 *              - Natural ( x > 0)              // needed

/* 
 * - Expression
 *   - Complex
 *   - Real
 *     - Integer
 *     - Rational
 *     - Prime integer
 *     - Natural number ( >= 0)
 *     - Irrational
 *  
 */

namespace Massive.Mathematics.Numerics
{
    public abstract class Skeleton
    {
        #region Constructors

        public Skeleton()
        {
            throw new NotImplementedException();
        }

        public Skeleton(int value)
            : this()
        {
            throw new NotImplementedException();
        }

        [CLSCompliant(false)]
        public Skeleton(uint value)
            : this()
        {
            throw new NotImplementedException();
        }

        public Skeleton(long value)
            : this()
        {
            throw new NotImplementedException();
        }

        [CLSCompliant(false)]
        public Skeleton(ulong value)
            : this()
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Unary arithmetic

        public static Skeleton operator -(Skeleton x)
        {
            throw new NotImplementedException();
        }

        public static Skeleton operator ++(Skeleton x)
        {
            throw new NotImplementedException();
        }

        public static Skeleton operator --(Skeleton x)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Arithmetic

        #region Addition

        public static Skeleton operator +(Skeleton x, Skeleton y)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Subtraction

        public static Skeleton operator -(Skeleton x, Skeleton y)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Multiplication

        public static Skeleton operator *(Skeleton x, Skeleton y)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Division

        public static Integer operator /(Skeleton x, Skeleton y)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Modulus

        public static Skeleton operator %(Skeleton x, Skeleton y)
        {
            throw new NotImplementedException();
        }

        public static int operator %(Skeleton x, int y)
        {
            throw new NotImplementedException();
        }

        public static long operator %(Skeleton x, long y)
        {
            throw new NotImplementedException();
        }

        #endregion

        #endregion

        #region In-place arithmetic

        public static Skeleton Negate(Skeleton x)
        {
            throw new NotImplementedException();
        }

        public static Skeleton Add(Skeleton x, Skeleton y)
        {
            throw new NotImplementedException();
        }

        public static Integer Subtract(Skeleton x, Skeleton y)
        {
            throw new NotImplementedException();
        }

        public static Skeleton Multiply(Skeleton x, Skeleton y)
        {
            throw new NotImplementedException();
        }

        public static Skeleton Divide(Skeleton x, Skeleton y)
        {
            throw new NotImplementedException();
        }

        public static Skeleton Modulus(Skeleton x, Skeleton y)
        {
            throw new NotImplementedException();
        }

        public static Skeleton Increment(Skeleton n)
        {
            throw new NotImplementedException();
        }

        public static Skeleton Decrement(Skeleton n)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Comparison

        #region Equality

        public static bool operator ==(Skeleton x, Skeleton y)
        {
            throw new NotImplementedException();
        }
        public static bool operator ==(Skeleton x, int y)
        {
            throw new NotImplementedException();
        }
        public static bool operator ==(Skeleton x, long y)
        {
            throw new NotImplementedException();
        }

        [CLSCompliant(false)]
        public static bool operator ==(Skeleton x, uint y)
        {
            throw new NotImplementedException();
        }
        [CLSCompliant(false)]
        public static bool operator ==(Skeleton x, ulong y)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Unequality

        public static bool operator !=(Skeleton x, Skeleton y)
        {
            throw new NotImplementedException();
        }
        public static bool operator !=(Skeleton x, int y)
        {
            throw new NotImplementedException();
        }
        public static bool operator !=(Skeleton x, long y)
        {
            throw new NotImplementedException();
        }

        [CLSCompliant(false)]
        public static bool operator !=(Skeleton x, uint y)
        {
            throw new NotImplementedException();
        }
        [CLSCompliant(false)]
        public static bool operator !=(Skeleton x, ulong y)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Less than

        public static bool operator <(Skeleton x, Skeleton y)
        {
            throw new NotImplementedException();
        }
        public static bool operator <(Skeleton x, int y)
        {
            throw new NotImplementedException();
        }
        public static bool operator <(Skeleton x, long y)
        {
            throw new NotImplementedException();
        }

        [CLSCompliant(false)]
        public static bool operator <(Skeleton x, uint y)
        {
            throw new NotImplementedException();
        }
        [CLSCompliant(false)]
        public static bool operator <(Skeleton x, ulong y)
        {
            throw new NotImplementedException();
        }

        public static bool operator <=(Skeleton x, Skeleton y)
        {
            throw new NotImplementedException();
        }
        public static bool operator <=(Skeleton x, int y)
        {
            throw new NotImplementedException();
        }
        public static bool operator <=(Skeleton x, long y)
        {
            throw new NotImplementedException();
        }

        [CLSCompliant(false)]
        public static bool operator <=(Skeleton x, uint y)
        {
            throw new NotImplementedException();
        }
        [CLSCompliant(false)]
        public static bool operator <=(Skeleton x, ulong y)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Greater than

        public static bool operator >(Skeleton x, Skeleton y)
        {
            throw new NotImplementedException();
        }
        public static bool operator >(Skeleton x, int y)
        {
            throw new NotImplementedException();
        }
        public static bool operator >(Skeleton x, long y)
        {
            throw new NotImplementedException();
        }

        [CLSCompliant(false)]
        public static bool operator >(Skeleton x, uint y)
        {
            throw new NotImplementedException();
        }
        [CLSCompliant(false)]
        public static bool operator >(Skeleton x, ulong y)
        {
            throw new NotImplementedException();
        }


        public static bool operator >=(Skeleton x, Skeleton y)
        {
            throw new NotImplementedException();
        }
        public static bool operator >=(Skeleton x, int y)
        {
            throw new NotImplementedException();
        }
        public static bool operator >=(Skeleton x, long y)
        {
            throw new NotImplementedException();
        }

        [CLSCompliant(false)]
        public static bool operator >=(Skeleton x, uint y)
        {
            throw new NotImplementedException();
        }
        [CLSCompliant(false)]
        public static bool operator >=(Skeleton x, ulong y)
        {
            throw new NotImplementedException();
        }

        #endregion

        #endregion

        #region Implicit casting

        public static implicit operator Skeleton(byte x)
        {
            throw new NotImplementedException();
        }

        public static implicit operator Skeleton(short x)
        {
            throw new NotImplementedException();
        }

        public static implicit operator Skeleton(int x)
        {
            throw new NotImplementedException();
        }

        public static implicit operator Skeleton(long x)
        {
            throw new NotImplementedException();
        }

        [CLSCompliant(false)]
        public static implicit operator Skeleton(uint x)
        {
            throw new NotImplementedException();
        }
        [CLSCompliant(false)]
        public static implicit operator Skeleton(ulong x)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Explicit casting

        public static explicit operator byte(Skeleton x)
        {
            throw new NotImplementedException();
        }

        public static explicit operator short(Skeleton x)
        {
            throw new NotImplementedException();
        }

        [CLSCompliant(false)]
        public static explicit operator ushort(Skeleton x)
        {
            throw new NotImplementedException();
        }

        public static explicit operator int(Skeleton x)
        {
            throw new NotImplementedException();
        }

        [CLSCompliant(false)]
        public static explicit operator uint(Skeleton x)
        {
            throw new NotImplementedException();
        }

        public static explicit operator long(Skeleton x)
        {
            throw new NotImplementedException();
        }

        [CLSCompliant(false)]
        public static explicit operator ulong(Skeleton x)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Overloading

        public override bool Equals(object obj)
        {
            throw new NotImplementedException();
        }

        public override int GetHashCode()
        {
            throw new NotImplementedException();
        }

        public override string ToString()
        {
            throw new NotImplementedException();
        }

        public string ToString(int radix)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Supplementary

        public Skeleton Clone()
        {
            throw new NotImplementedException();
        }

        #endregion

    }

}
