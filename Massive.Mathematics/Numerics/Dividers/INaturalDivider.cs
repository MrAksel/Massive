using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Massive.Mathematics.Numerics.Dividers
{
    public interface INaturalDivider
    {
        /// <summary>
        /// Divides dividend by divisor and places quotient in dividend
        /// </summary>
        /// <param name="dividend"></param>
        /// <param name="divisor"></param>
        void Divide(Natural dividend, Natural divisor);
        /// <summary>
        /// Divides <paramref name="dividend"/> by <paramref name="<divisor"/> and places quotient in <paramref name="quotient"/>
        /// </summary>
        /// <param name="dividend"></param>
        /// <param name="divisor"></param>
        /// <param name="quotient"></param>
        void Divide(Natural dividend, Natural divisor, out Natural quotient);

        /// <summary>
        /// Divides <paramref name="dividend"/> by <paramref name="divisor"/>, places quotient in in <paramref name="dividend"/>, and places remainder in <paramref name="divisor"/>
        /// </summary>
        /// <param name="dividend"></param>
        /// <param name="divisor"></param>
        void DivRem(Natural dividend, Natural divisor);
        /// <summary>
        /// Divides <paramref name="dividend"/> by <paramref name="divisor"/>, places quotient in in <paramref name="quotient"/>, and places remainder in <paramref name="remainder"/>
        /// </summary>
        /// <param name="dividend"></param>
        /// <param name="divisor"></param>
        void DivRem(Natural dividend, Natural divisor, out Natural quotient, out Natural remainder);

        /// <summary>
        /// Divides <paramref name="dividend"/> by <paramref name="divisor"/> and places remainder in <paramref name="dividend"/>
        /// </summary>
        /// <param name="dividend"></param>
        /// <param name="divisor"></param>
        void Remainder(Natural dividend, Natural divisor);
        /// <summary>
        /// Divides <paramref name="dividend"/> by <paramref name="divisor"/> and places remainder in <paramref name="remainder"/>
        /// </summary>
        /// <param name="dividend"></param>
        /// <param name="divisor"></param>
        /// <param name="remainder"></param>
        void Remainder(Natural dividend, Natural divisor, out Natural remainder);
    }
}
