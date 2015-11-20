using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Massive.Mathematics.Numerics.Multipliers
{
    public class DefaultMultiplierManager : IMultiplierManager
    {
        static INaturalMultiplier defaultMultiplier = new LongMultiplier();

        public INaturalMultiplier GetDefaultMultiplier()
        {
            return defaultMultiplier;
        }

        public INaturalMultiplier GetMultiplier(Natural n1, Natural n2)
        {
            return GetDefaultMultiplier();
        }
    }
}
