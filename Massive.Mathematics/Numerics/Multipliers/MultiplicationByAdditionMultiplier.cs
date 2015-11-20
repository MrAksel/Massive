using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Massive.Mathematics.Numerics.Multipliers
{
    public class MultiplicationByAdditionMultiplier : INaturalMultiplier
    {
        public void Multiply(Natural factor1, Natural factor2)
        {
            if (factor1 == Natural.Zero || factor2 == Natural.Zero)
            {
                factor1.SetValue(Natural.Zero);
            }
            else if (factor1 == Natural.One)
            {
                factor1.SetValue(factor2);
            }
            else
            {
                Natural increase = factor1.Clone();
                Natural decrease = factor2.Clone();

                factor1.PushLength(factor1.usedDigits * factor2.usedDigits);

                while (decrease > 1) // We already have 1 factor1
                {
                    Natural.Add(factor1, increase);
                    Natural.Decrement(decrease);
                }

                factor1.PopLength();
            }
        }

        public void Multiply(Natural factor1, Natural factor2, out Natural product)
        {
            if (factor1 == Natural.Zero || factor2 == Natural.Zero)
            {
                product = Natural.Zero;
            }
            else if (factor1 == Natural.One)
            {
                product = factor2.Clone();
            }
            else
            {
                Natural counter = factor2.Clone();
                product = factor1.Clone();

                product.PushLength(factor1.usedDigits * factor2.usedDigits);

                while (counter > 1) // We already have 1 factor1
                {
                    Natural.Add(product, factor1);
                    Natural.Decrement(counter);
                }

                product.PopLength();
            }
        }
    }
}
