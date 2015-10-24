using Massive.Collections;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Diagnostics;

namespace Massive.Testing.Collections
{
    [TestClass]
    public class IndexedArraySegmentTests
    {

        [TestMethod]
        public void TestSegmentShiftLeft()
        {
                           // from     here         to         here
            byte[] arr = { 0xF1, 0xA4, 0xB7, 0x09, 0x31, 0xCC, 0x67, 0xAA, 0xBB, 0xFE };
            byte[] smn = { 0xB7, 0x09, 0x31, 0xCC, 0x67 };

            byte[] s2 = { 0x31, 0xCC, 0x67, 0x00, 0x00 };
            byte[] s6 = { 0x00, 0x00, 0x00, 0x00, 0x00 };

            IndexedArraySegment<byte> seg = new IndexedArraySegment<byte>(arr.Duplicate(), 2, 5);

            CollectionAssert.AreEqual(smn, seg.ToArray());

            seg.ShiftLeft(2);

            CollectionAssert.AreEqual(s2, seg.ToArray());

            seg = new IndexedArraySegment<byte>(arr.Duplicate(), 2, 5);
            seg.ShiftLeft(6);

            CollectionAssert.AreEqual(s6, seg.ToArray());
        }

        [TestMethod]
        public void TestSegmentShiftRight()
        {
                     // from     here         to                           here
            byte[] arr = { 0xF1, 0xA4, 0xB7, 0x09, 0x31, 0xCC, 0x67, 0xAA, 0xBB, 0xFE };
            byte[] smn = { 0xA4, 0xB7, 0x09, 0x31, 0xCC, 0x67, 0xAA, 0xBB };

            byte[] s3 = { 0x00, 0x00, 0x00, 0xA4, 0xB7, 0x09, 0x31, 0xCC };
            byte[] s6 = { 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xA4, 0xB7 };

            IndexedArraySegment<byte> seg = new IndexedArraySegment<byte>(arr.Duplicate(), 1, 8);

            byte[] res = seg.ToArray();

            Debug.WriteLine("Testing ToArray()");
            CollectionAssert.AreEqual(smn, res);

            Debug.WriteLine("Testing rightshift 3");
            seg.ShiftRight(3);
            res = seg.ToArray();
            Debug.WriteLine("{0}\n{1}", BitConverter.ToString(res), BitConverter.ToString(s3));
            CollectionAssert.AreEqual(s3, res);

            seg = new IndexedArraySegment<byte>(arr.Duplicate(), 1, 8);
            Debug.WriteLine("Testing rightshift 6");
            seg.ShiftRight(6);
            res = seg.ToArray();
            Debug.WriteLine("{0}\n{1}", BitConverter.ToString(res), BitConverter.ToString(s6));
            CollectionAssert.AreEqual(s6, res);
        }
    }
}
