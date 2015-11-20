using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Massive.Mathematics.Numerics.Subtracters
{
    public interface INaturalSubtracter
    {
        void Subtract(Natural minuend, Natural subtrahend);
        void Subtract(Natural minuend, Natural subtrahend, out Natural difference);
    }
}
