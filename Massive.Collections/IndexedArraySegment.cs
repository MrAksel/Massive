using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Massive.Collections
{
    // Thank you Microsoft for a great start (and time saver). http://referencesource.microsoft.com/#mscorlib/system/arraysegment.cs
    /// <summary>
    /// Wrapper over a part of an array. Provides some extra functionality.
    /// All data is stored and treated as little endian (least significant first)
    /// </summary>
    public class IndexedArraySegment<T> : IList<T>, IReadOnlyList<T>
    {
        private T[] _array;
        private int _offset;
        private int _count;

        public IndexedArraySegment(T[] array)
        {
            if (array == null)
                throw new ArgumentNullException("array");

            _array = array;
            _offset = 0;
            _count = array.Length;
        }

        public IndexedArraySegment(T[] array, int offset, int count)
        {
            if (array == null)
                throw new ArgumentNullException("array");
            if (offset < 0)
                throw new ArgumentOutOfRangeException("offset");
            if (count < 0)
                throw new ArgumentOutOfRangeException("count");
            if (offset + count > array.Length)
                throw new ArgumentException();

            _array = array;
            _offset = offset;
            _count = count;
        }

        public T[] Array
        {
            get
            {
                return _array;
            }
        }

        public int Offset
        {
            get
            {
                return _offset;
            }
        }

        public int Count
        {
            get
            {
                return _count;
            }
        }

        public override int GetHashCode()
        {
            return null == _array
                        ? 0
                        : _array.GetHashCode() ^ _offset ^ _count;
        }

        public override bool Equals(Object obj)
        {
            if (obj is IndexedArraySegment<T>)
                return Equals((IndexedArraySegment<T>)obj);
            else
                return false;
        }

        public bool Equals(IndexedArraySegment<T> obj)
        {
            return obj._array == _array && obj._offset == _offset && obj._count == _count;
        }

        public static bool operator ==(IndexedArraySegment<T> a, IndexedArraySegment<T> b)
        {
            return a.Equals(b);
        }

        public static bool operator !=(IndexedArraySegment<T> a, IndexedArraySegment<T> b)
        {
            return !(a == b);
        }


        public static implicit operator IndexedArraySegment<T>(T[] a)
        {
            return new IndexedArraySegment<T>(a, 0, a.Length);
        }

        public T[] ToArray()
        {
            T[] t = new T[_count];
            System.Array.Copy(_array, _offset, t, 0, _count);
            return t;
        }

        public void ExpandWindow(int newSize)
        {
            if (_count >= newSize)
                return;

            int oldsize = _count;
            // Resize array if neccessary
            int missing = newSize - (_array.Length - _offset);
            if (missing > 0)
            {
                System.Array.Resize(ref _array, _array.Length + missing);
            }

            System.Array.Clear(_array, _offset + oldsize, newSize - oldsize - missing);
            _count = newSize;
        }

        public void ShrinkWindow(int newSize)
        {
            if (_count <= newSize)
                return;
            if (newSize < 0)
                throw new ArgumentException();

            _count = newSize;
        }

        public override string ToString()
        {
            return string.Join(" ", ToArray().Select(v => v.ToString()));
        }

        public void ShiftContentsLeft(int elements)
        {

            // 01 | 02 03 04 | 05 06      // elements = 3, missing = 1
            // 01   02 03 04 | 05 06    | // offset += elements
            // 01   02 03 04 | 00 00 00 | // clear oldoffset + count, elements - missing

            // 01 | 02 03   04 | 05 06    // elements = 2, missing = 0
            // 01   02 03 | 04   05 06 |  // offset += elements
            // 01   02 03 | 04   00 00 |  // clear oldoffset + count, elements - missing

            Debug.Write(string.Format("LShift {0} : {1} -> ", elements, ToString()));

            int oldoffset = _offset;
            int missing = elements - (_array.Length - _offset - _count);
            _offset += elements;

            if (missing > 0)
            {
                Debug.WriteLine("Missing {0} elements - growing array", missing);
                System.Array.Resize(ref _array, _array.Length + missing);
            }

            System.Array.Clear(_array, oldoffset + _count, elements - missing);

            Debug.WriteLine(ToString());
        }

        public void ShiftContentsRight(int elements)
        {
            int oldoffset = _offset;

            Debug.Write(string.Format("RShift {0} : {1} -> ", elements, ToString()));

            if (_offset - elements < 0)
            {
                int missing = elements - _offset;

                _offset -= elements - missing; // offset = 0
                System.Array.Copy(_array, oldoffset, _array, oldoffset + missing, _count - missing);
                System.Array.Clear(_array, 0, oldoffset + missing);
            }
            else
            {
                // shift right by 4

                //  E5   7F   10   C2   6F   01   A2 | 01   02   03   04   05   06 |    offset = 6, count = 6
                //  E5   7F   10 | C2   6F   01   A2   01   02 | 03   04   05   06      offset -= elements
                //  ............ | 00   00   00   00   01   02 |                        clear elements from offset - elements 

                _offset -= elements;
                System.Array.Clear(_array, _offset, elements);
            }
            Debug.WriteLine(ToString());
        }

        public IndexedArraySegment<T> GetSegment(int offset, int count)
        {
            return new IndexedArraySegment<T>(_array, _offset + offset, count);
        }

        #region IList<T>

        public T this[int index]
        {
            get
            {
                if (_array == null)
                    throw new InvalidOperationException();
                if (index < 0 || index >= _count)
                    throw new ArgumentOutOfRangeException("index");

                return _array[_offset + index];
            }

            set
            {
                if (_array == null)
                    throw new InvalidOperationException();
                if (index < 0 || index >= _count)
                    throw new ArgumentOutOfRangeException("index");

                _array[_offset + index] = value;
            }
        }

        int IList<T>.IndexOf(T item)
        {
            if (_array == null)
                throw new InvalidOperationException();

            int index = System.Array.IndexOf<T>(_array, item, _offset, _count);

            return index >= 0 ? index - _offset : -1;
        }

        void IList<T>.Insert(int index, T item)
        {
            throw new NotSupportedException();
        }

        void IList<T>.RemoveAt(int index)
        {
            throw new NotSupportedException();
        }
        #endregion


        #region ICollection<T>
        bool ICollection<T>.IsReadOnly
        {
            get
            {
                // the indexer setter does not throw an exception although IsReadOnly is true.
                // This is to match the behavior of arrays.
                return true;
            }
        }

        void ICollection<T>.Add(T item)
        {
            throw new NotSupportedException();
        }

        void ICollection<T>.Clear()
        {
            throw new NotSupportedException();
        }

        bool ICollection<T>.Contains(T item)
        {
            if (_array == null)
                throw new InvalidOperationException();

            int index = System.Array.IndexOf<T>(_array, item, _offset, _count);

            return index >= 0;
        }

        void ICollection<T>.CopyTo(T[] array, int arrayIndex)
        {
            if (_array == null)
                throw new InvalidOperationException();

            System.Array.Copy(_array, _offset, array, arrayIndex, _count);
        }

        bool ICollection<T>.Remove(T item)
        {
            throw new NotSupportedException();
        }
        #endregion

        #region IEnumerable<T>
        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            if (_array == null)
                throw new InvalidOperationException();

            return new ArraySegmentEnumerator(this);
        }
        #endregion

        #region IEnumerable
        IEnumerator IEnumerable.GetEnumerator()
        {
            if (_array == null)
                throw new InvalidOperationException();

            return new ArraySegmentEnumerator(this);
        }
        #endregion

        [Serializable]
        private sealed class ArraySegmentEnumerator : IEnumerator<T>
        {
            private T[] _array;
            private int _start;
            private int _end;
            private int _current;

            internal ArraySegmentEnumerator(IndexedArraySegment<T> arraySegment)
            {
                _array = arraySegment._array;
                _start = arraySegment._offset;
                _end = _start + arraySegment._count;
                _current = _start - 1;
            }

            public bool MoveNext()
            {
                if (_current < _end)
                {
                    _current++;
                    return (_current < _end);
                }
                return false;
            }

            public T Current
            {
                get
                {
                    if (_current < _start) throw new InvalidOperationException();
                    if (_current >= _end) throw new InvalidOperationException();
                    return _array[_current];
                }
            }

            object IEnumerator.Current
            {
                get
                {
                    return Current;
                }
            }

            void IEnumerator.Reset()
            {
                _current = _start - 1;
            }

            public void Dispose()
            {
            }
        }
    }
}
