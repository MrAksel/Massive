using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Massive.Mathematics.Numerics.Multipliers
{
    public interface INaturalMultiplier
    {
        void Multiply(Natural factor1, Natural factor2);
        void Multiply(Natural factor1, Natural factor2, out Natural product);
    }
}
