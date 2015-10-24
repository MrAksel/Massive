using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics;

namespace Massive.Testing
{
    [TestClass]
    public class HelperTests
    {
        [TestMethod]
        public void TestUIntToByteArrayBE()
        {
            uint[] ints = { 0xF1, 0xE5A19812, 0x91B30C00 };
            byte[] expt = { 0x00, 0x00, 0x00, 0xF1, 0xE5, 0xA1, 0x98, 0x12, 0x91, 0xB3, 0x0C, 0x00 };

            CollectionAssert.AreEqual(expt, Helper.ConvertUIntArrayToByteArrayBE(ints));
        }
    
        [TestMethod]
        public void TestUIntToByteArrayLE()
        {
            uint[] ints = { 0xF1, 0xE5A19812, 0x91B30C00 };
            byte[] expt = { 0xF1, 0x00, 0x00, 0x00, 0x12, 0x98, 0xA1, 0xE5, 0x00, 0x0C, 0xB3, 0x91 };

            CollectionAssert.AreEqual(expt, Helper.ConvertUIntArrayToByteArrayLE(ints));
        }

        [TestMethod]
        public void TestByteToUIntArrayBE()
        {
            byte[] bytes = { 0x00, 0x00, 0x00, 0xF1, 0xE5, 0xA1, 0x98, 0x12, 0x91, 0xB3, 0x0C, 0x00 };
            uint[] expct = { 0xF1, 0xE5A19812, 0x91B30C00 };

            CollectionAssert.AreEqual(expct, Helper.ConvertByteArrayToUIntArrayBE(bytes));

            bytes = new byte[] { 0x10, 0x0F, 0xE4, 0xFF, 0x00, 0x21, 0x5F };
            expct = new uint[] { 0x100FE4FF, 0x00215F00 };
            
            CollectionAssert.AreEqual(expct, Helper.ConvertByteArrayToUIntArrayBE(bytes));
        }

        [TestMethod]
        public void TestByteToUIntArrayLE()
        {
            byte[] bytes = { 0x00, 0x00, 0x00, 0xF1, 0xE5, 0xA1, 0x98, 0x12, 0x91, 0xB3, 0x0C, 0x00 };
            uint[] expct = { 0xF1000000, 0x1298A1E5, 0x000CB391 };

            CollectionAssert.AreEqual(expct, Helper.ConvertByteArrayToUIntArrayLE(bytes));

            bytes = new byte[] { 0x10, 0x0F, 0xE4, 0xFF, 0x00, 0x21, 0x5F };
            expct = new uint[] { 0xFFE40F10, 0x005F2100 };

            CollectionAssert.AreEqual(expct, Helper.ConvertByteArrayToUIntArrayLE(bytes));
        }
    }
}
