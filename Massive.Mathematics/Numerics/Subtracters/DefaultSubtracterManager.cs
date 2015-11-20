using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Massive.Mathematics.Numerics.Subtracters
{
    class DefaultSubtracterManager : ISubtracterManager
    {
        private static INaturalSubtracter defaultSubtracter = new ComplementSubtracter();

        public INaturalSubtracter GetDefaultSubtracter()
        {
            return defaultSubtracter;
        }

        public INaturalSubtracter GetSubtracter(Natural n1, Natural n2)
        {
            return GetDefaultSubtracter();
        }
    }
}
