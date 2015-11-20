using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Massive.Mathematics.Numerics.Adders
{
    public class DefaultAdderManager : IAdderManager
    {
        static StandardAdder defaultAdder = new StandardAdder();

        public INaturalAdder GetDefaultAdder()
        {
            return defaultAdder;
        }

        public INaturalAdder GetAdder(Natural n1, Natural n2)
        {
            return GetDefaultAdder();
        }
    }
}
