﻿using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Numerics;

namespace Massive.Mathematics.Testing
{
    [TestClass]
    public class NaturalTests
    {
        ulong[] samples = new ulong[] { 0, 1, 2, 5, 10, 100000, int.MaxValue, uint.MaxValue - 1, ulong.MaxValue / 2, ulong.MaxValue };

        [TestMethod]
        public void AddTest()
        {
            Natural.DefaultNumberSize = 4;

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
            Natural.DefaultNumberSize = 4;

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
            Natural.DefaultNumberSize = 4;

            foreach (ulong l in samples)
            {
                foreach (ulong r in samples)
                {
                    Natural ll = new Natural(l);
                    Natural rr = new Natural(r);

                    BigInteger bl = new BigInteger(l);
                    BigInteger br = new BigInteger(r);

                    Assert.AreEqual(bl * br, (BigInteger)(ll * rr));
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
            Natural.DefaultNumberSize = 4;

            int[] powers = { 0, 1, 2, 3, 4, 5 };
            foreach(ulong l in samples)
            {
                foreach (int p in powers)
                {
                    Natural ll = new Natural(l);
                    BigInteger bl = new BigInteger(l);

                    BigInteger rs = (BigInteger)(BigInteger)Natural.Pow(ll, p);
                    if (BigInteger.Pow(bl, p) != rs)
                    {
                        Assert.Fail(string.Format("{0} ^ {1} = {2}", l, p, rs));
                    }
                    // Assert.AreEqual(BigInteger.Pow(bl, p), (BigInteger)Natural.Pow(ll, p));
                }
            }
        }

        [TestMethod]
        public void DivisionTest()
        {
            Natural.DefaultNumberSize = 4;

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
            Natural.DefaultNumberSize = 4;

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
