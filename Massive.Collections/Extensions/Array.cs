using System;
using System.Diagnostics;

namespace Massive.Collections
{
    public static class ArrayExtensions
    {
        public static T[] ShiftLeft<T>(this T[] arr, int elements)
        {
            Debug.WriteLine("Shift array left by {0} elements", elements);

            T[] newArr = new T[arr.Length];
            Array.Copy(arr, elements, newArr, 0, arr.Length - elements);

            return newArr;
        }

        public static T[] ShiftRight<T>(this T[] arr, int elements)
        {
            Debug.WriteLine("Shift array right by {0} elements. New length = {1} + {0} = {2}", elements, arr.Length, arr.Length + elements);

            T[] newArr = new T[arr.Length + elements];
            Array.Copy(arr, 0, newArr, elements, arr.Length);

            return newArr;
        }

        public static T[] Duplicate<T>(this T[] arr)
        {
            T[] n = new T[arr.Length];
            Array.Copy(arr, n, n.Length);

            return n;
        }

        public static IndexedArraySegment<T> GetSegment<T>(this T[] arr, int offset, int count)
        {
            return new IndexedArraySegment<T>(arr, offset, count);
        }
    }
}
