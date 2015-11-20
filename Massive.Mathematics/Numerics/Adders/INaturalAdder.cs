using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Massive.Mathematics.Numerics.Adders
{
    public interface INaturalAdder
    {
        void Add(Natural addend1, Natural addend2);
        void Add(Natural addend1, Natural addend2, out Natural sum);
    }
}
