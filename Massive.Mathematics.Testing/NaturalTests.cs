using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Numerics;

namespace Massive.Mathematics.Testing
{
    [TestClass]
    public class NaturalTests
    {
        [TestMethod]
        public void AddTest()
        {
            Natural.default_array_size = 4;

            uint[] left = new uint[] { 0, 1, 2, 5, 10, 100000, int.MaxValue, uint.MaxValue - 1 };
            uint[] right = new uint[] { 0, 1, 2, 5, 10, 100000, int.MaxValue, uint.MaxValue - 1 };

            foreach (uint l in left)
            {
                foreach (uint r in right)
                {
                    Natural ll = new Natural(l);
                    Natural rr = new Natural(r);

                    Assert.AreEqual((ulong)l + r, ll + rr);
                }
            }
        }

        [TestMethod]
        public void SubtractTest()
        {
            Natural.default_array_size = 4;

            uint[] left = new uint[] { 0, 1, 2, 5, 10, 100000, int.MaxValue, uint.MaxValue - 1 };
            uint[] right = new uint[] { 0, 1, 2, 5, 10, 100000, int.MaxValue, uint.MaxValue - 1 };

            foreach (uint l in left)
            {
                foreach (uint r in right)
                {
                    if (r > l)
                        continue;

                    Natural ll = new Natural(l);
                    Natural rr = new Natural(r);

                    Assert.AreEqual((ulong)l - r, ll - rr);
                }
            }
        }

        [TestMethod]
        public void MultiplyTest()
        {
            Natural.default_array_size = 4;

            uint[] left = new uint[] { 0, 1, 2, 5, 10, 100000, int.MaxValue, uint.MaxValue - 1 };
            uint[] right = new uint[] { 0, 1, 2, 5, 10, 100000, int.MaxValue, uint.MaxValue - 1 };

            foreach (uint l in left)
            {
                foreach (uint r in right)
                {
                    Natural ll = new Natural(l);
                    Natural rr = new Natural(r);

                    Assert.AreEqual((ulong)l * r, ll * rr);
                }
            }
        }

        [TestMethod]
        public void DivisionTest()
        {
            Natural.default_array_size = 4;

            uint[] left = new uint[] { 0, 1, 2, 5, 10, 100000, int.MaxValue, uint.MaxValue };
            uint[] right = new uint[] { 0, 1, 2, 5, 10, 100000, int.MaxValue, uint.MaxValue };

            foreach (uint l in left)
            {
                foreach (uint r in right)
                {
                    if (r == 0)
                        continue;

                    Natural ll = new Natural(l);
                    Natural rr = new Natural(r);

                    Assert.AreEqual((ulong)l / r, ll / rr);
                }
            }
        }

        [TestMethod]
        public void ToStringTest()
        {
            Natural.default_array_size = 4;

            Natural[] left = new Natural[] { 0, 1, 2, 5, 10, 100, 1000, 1000000, int.MaxValue, uint.MaxValue, (Natural)uint.MaxValue * uint.MaxValue * 0 };

            foreach (Natural l in left)
            {
                Console.WriteLine(l.ToString());
            }
        }

        [TestMethod]
        public void ToBigIntTest()
        {
            Random r = new Random();
            for (int i = 0; i < 10; i++)
            {
                int rnd = r.Next(50);
                Natural n = rnd;
                BigInteger bi = rnd;

                Console.WriteLine("{0} | {1}", n, bi);
            }
        }
    }
}
