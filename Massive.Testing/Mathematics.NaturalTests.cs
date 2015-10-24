using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Numerics;
using Massive.Mathematics.Numerics;
using System.Diagnostics;

namespace Massive.Testing.Mathematics
{
    [TestClass]
    public class NaturalTests
    {
        int defnumsize = 1;
        ulong[] samples = new ulong[] { 0, 1, 2, 5, 10, 100000, int.MaxValue, uint.MaxValue - 1, ulong.MaxValue / 2, ulong.MaxValue };

        [TestMethod]
        public void AddTest()
        {
            Natural.DefaultNumberSize = defnumsize;

            foreach (ulong l in samples)
            {
                foreach (ulong r in samples)
                {
                    Natural ll = new Natural(l);
                    Natural rr = new Natural(r);

                    BigInteger bl = new BigInteger(l);
                    BigInteger br = new BigInteger(r);

                    Assert.AreEqual(bl + br, (BigInteger)(ll + rr));
                }
            }

            for (int i = 2; i < 10; i++)
            {
                for (int p = 32; p < 256; p += 12)
                {
                    BigInteger b = BigInteger.Pow(i, p);
                    BigInteger l = BigInteger.Pow(i + 1, p - 1) * 7;

                    BigInteger r = b + l;
                    Natural n = (Natural)b + (Natural)l;

                    Assert.AreEqual(r, (BigInteger)n);
                }
            }
        }

        [TestMethod]
        public void SubtractTest()
        {
            Natural.DefaultNumberSize = defnumsize;

            foreach (ulong l in samples)
            {
                foreach (ulong r in samples)
                {
                    if (r > l)
                        continue;

                    Natural ll = new Natural(l);
                    Natural rr = new Natural(r);

                    BigInteger bl = new BigInteger(l);
                    BigInteger br = new BigInteger(r);

                    Assert.AreEqual(bl - br, (BigInteger)(ll - rr));
                }
            }
        }

        [TestMethod]
        public void MultiplyTest()
        {
            Natural.DefaultNumberSize = defnumsize;

            foreach (ulong l in samples)
            {
                foreach (ulong r in samples)
                {
                    Natural ll = new Natural(l);
                    Natural rr = new Natural(r);

                    BigInteger bl = new BigInteger(l);
                    BigInteger br = new BigInteger(r);

                    BigInteger bres = bl * br;
                    BigInteger nres = (BigInteger)(ll * rr);

                    Debug.WriteLine("{0:X} * {1:X} = {2:X} = {3:X}", l, r, bres, nres);

                    Assert.AreEqual(bres, nres);
                }
            }

            Natural pow = 1;
            BigInteger bi = BigInteger.Pow(2147483647, 5);
            pow *= 2147483647;
            pow *= 2147483647;
            pow *= 2147483647;
            pow *= 2147483647;
            pow *= 2147483647;
            if (bi != (BigInteger)pow)
                Assert.Fail("Pow. Got {0}, should be {1}", (BigInteger)pow, bi);
        }



        [TestMethod]
        public void LargeNumTest()
        {
            Natural.DefaultNumberSize = defnumsize;

            int[] powers = { 0, 1, 2, 3, 4, 5 };
            foreach (ulong l in samples)
            {
                foreach (int p in powers)
                {
                    Natural ll = new Natural(l);
                    BigInteger bl = new BigInteger(l);

                    BigInteger rs = (BigInteger)Natural.Pow(ll, p);
                    Assert.AreEqual(BigInteger.Pow(bl, p), rs);
                }
            }
        }

        [TestMethod]
        public void DivisionTest()
        {
            Natural.DefaultNumberSize = defnumsize;

            foreach (ulong l in samples)
            {
                foreach (ulong r in samples)
                {
                    Natural ll = new Natural(l);
                    Natural rr = new Natural(r);

                    BigInteger bl = new BigInteger(l);
                    BigInteger br = new BigInteger(r);

                    if (r == 0)
                        continue;

                    Assert.AreEqual(bl / br, (BigInteger)(ll / rr));
                }
            }
        }

        [TestMethod]
        public void ToStringTest()
        {
            Natural.DefaultNumberSize = defnumsize;

            foreach (ulong l in samples)
            {
                Natural n = new Natural(l);
                BigInteger b = new BigInteger(l);

                string s = n.ToString();

                Assert.AreEqual(BigInteger.Parse(s), b);
            }
        }

        [TestMethod]
        public void ConversionTest()
        {
            Natural.DefaultNumberSize = defnumsize;

            foreach (ulong i in samples)
            {
                for (int pow = 412; pow < 440; pow++)
                {
                    BigInteger p = BigInteger.Pow(i, pow);

                    Natural l = (Natural)p;

                    BigInteger r = (BigInteger)l;
                    Assert.AreEqual(p, r);
                }
            }
        }
    }
}
