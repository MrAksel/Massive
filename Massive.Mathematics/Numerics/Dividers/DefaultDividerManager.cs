using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Massive.Mathematics.Numerics.Dividers
{
    public class DefaultDividerManager : IDividerManager
    {
        static INaturalDivider defaultDivider = new RecursiveDivider();

        public INaturalDivider GetDefaultDivider()
        {
            return defaultDivider;
        }

        public INaturalDivider GetDivider(Natural n1, Natural n2)
        {
            return GetDefaultDivider();
        }
    }
}
