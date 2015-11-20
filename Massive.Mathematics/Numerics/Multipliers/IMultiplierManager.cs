using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Massive.Mathematics.Numerics.Multipliers
{
    public interface IMultiplierManager
    {
        INaturalMultiplier GetDefaultMultiplier();

        INaturalMultiplier GetMultiplier(Natural n1, Natural n2);
    }
}
