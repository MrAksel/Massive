using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Massive.Mathematics.Numerics.Subtracters
{
    public interface ISubtracterManager
    {
        INaturalSubtracter GetDefaultSubtracter();

        INaturalSubtracter GetSubtracter(Natural n1, Natural n2); // Should we pass the numbers, the base-32 logarithm, base-2 logarithm, or nothing?
    }
}
