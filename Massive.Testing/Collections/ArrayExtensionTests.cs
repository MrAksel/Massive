using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Massive.Collections;

namespace Massive.Testing.Collections
{
    [TestClass]
    public class Collections
    {

        [TestMethod]
        public void TestShiftLeft()
        {
            byte[] test = { 4, 12, 64, 94, 231 };
            byte[] expc = { 12, 64, 94, 231, 0 };
            byte[] epc2 = { 64, 94, 231, 0, 0 };

            byte[] res = test.ShiftLeft(1);
            CollectionAssert.AreEqual(expc, res);

            res = test.ShiftLeft(2);
            CollectionAssert.AreEqual(epc2, res);
        }

        [TestMethod]
        public void TestShiftRight()
        {
            byte[] test = { 4, 12, 64, 94, 231 };
            byte[] expc = { 0, 4, 12, 64, 94, 231 };
            byte[] epc2 = { 0, 0, 0, 4, 12, 64, 94, 231 };

            byte[] res = test.ShiftRight(1);
            CollectionAssert.AreEqual(expc, res);

            res = test.ShiftRight(3);
            CollectionAssert.AreEqual(epc2, res);
        }
    }
}
