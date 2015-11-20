using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Massive.Mathematics.Numerics.Adders
{
    public interface IAdderManager
    {
        INaturalAdder GetDefaultAdder();

        INaturalAdder GetAdder(Natural n1, Natural n2);
    }
}
