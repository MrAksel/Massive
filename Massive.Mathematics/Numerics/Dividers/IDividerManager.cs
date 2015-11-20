using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Massive.Mathematics.Numerics.Dividers
{
    public interface IDividerManager
    {
        INaturalDivider GetDefaultDivider();

        INaturalDivider GetDivider(Natural n1, Natural n2);
    }
}
