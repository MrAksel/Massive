using System;
using System.Collections;
using System.Collections.Generic;

namespace Massive.Collections
{
    public class LargeList<T> : IEnumerable<T>
    {
        private int sub_array_size;

        T[][] data;

        protected LargeList()
        {
            data = null;
            sub_array_size = 0;
        }

        public LargeList(int sub_length)
            :this(sub_length, 1)
        {
        }

        public LargeList(int sub_length, int array_count)
        {
            sub_array_size = sub_length;
            data = new T[array_count][];
            for (int i = 0; i < array_count; i++)
                data[i] = new T[sub_length];
        }

        public LargeList(T[] array, int sub_length)
        {
            sub_array_size = sub_length;
            int lng = (array.Length > 0 ? (array.Length - 1) / sub_length + 1 : 0);
            long cIndex = 0;
            data = new T[lng][];

            for (int i = 0; i < lng; i++)
            {
                data[i] = new T[sub_length];

                long max = Math.Min(array.LongLength - cIndex, sub_length);
                Array.Copy(array, cIndex, data[i], 0, max);
                cIndex += max;
            }
        }


        public T this[long index]
        {
            get
            {
                return Get(index);
            }
            set
            {
                Put(index, value);
            }
        }

        public long Length
        {
            get
            {
                return data.LongLength * sub_array_size;
            }
        }

        public long MaxLength
        {
            get
            {
                return (long)int.MaxValue * sub_array_size;
            }
        }


        public void Put(long index, T item)
        {
            if (index >= MaxLength)
                throw new ArgumentOutOfRangeException("index cannot be larger than or equal to MaxLength.");

            if (Length < index)
                Grow(index);

            data[index / sub_array_size][index % sub_array_size] = item;
        }

        public T Get(long index)
        {
            if (index >= MaxLength)
                throw new ArgumentOutOfRangeException("index cannot be larger than or equal to MaxLength.");

            if (Length < index)
                Grow(index);

            return data[index / sub_array_size][index % sub_array_size];
        }


        public void Grow(long length)
        {
            if (length > MaxLength)
                throw new ArgumentOutOfRangeException("length cannot be larger than MaxLength.");

            int arrc = (int)(length / sub_array_size);
            int oldc = data.Length;

            if (arrc > oldc)
            {
                Array.Resize(ref data, arrc);
                for (int i = oldc; i < arrc; i++)
                {
                    data[i] = new T[sub_array_size];
                }
            }
        }

        public void Shrink(long length)
        {
            if (length > MaxLength)
                throw new ArgumentOutOfRangeException("length cannot be larger than MaxLength.");

            int arrc = (int)(length / sub_array_size);
            int oldc = data.Length;

            if (arrc < oldc)
            {
                Array.Resize(ref data, arrc);
            }
        }

        public void Resize(long length)
        {
            if (length > MaxLength)
                throw new ArgumentOutOfRangeException("length cannot be larger than MaxLength.");

            int arrc = (int)(length / sub_array_size);
            if (arrc == 0)
                arrc = 1;
            int oldc = data.Length;

            if (arrc > oldc)
            {
                Array.Resize(ref data, arrc);
                for (int i = oldc; i < arrc; i++)
                {
                    data[i] = new T[sub_array_size];
                }
            }
            else if (arrc < oldc)
            {
                Array.Resize(ref data, arrc);
            }
        }


        public void ShiftLeft(int elements)
        {
            throw new NotImplementedException();
        }

        public void ShiftRight(int elements)
        {
            throw new NotImplementedException();
        }

        /*
        public long IndexOf(T value)
        {
            long index = 0;
            foreach (T[] t in data)
            {
                for (int i = 0; i < t.Length; i++)
                {
                    if (t[i].Equals(value))
                        return index + i;
                }
                index += t.Length;
            }
            return -1L;
        }
        */

        public LargeList<T> Clone()
        {
            LargeList<T> n = new LargeList<T>(sub_array_size, data.Length);
            for (int i = 0; i < data.Length; i++)
                Array.Copy(data[i], n.data[i], sub_array_size);

            return n;
        }

        public void Clear()
        {
            Array.Resize(ref data, 1);
            for (int i = 0; i < data[0].Length; i++)
            {
                data[0][i] = default(T);
            }
        }

        public T[] ToArray()
        {
            return ToArray(Length);
        }

        public T[] ToArray(long length)
        {
            T[] a = new T[length];
            long c = 0;
            long i = 0;
            while (c < length)
            {
                long m = Math.Min(length - c, sub_array_size);
                Array.Copy(data[i], a, m);
                i++;
                c += m;
            }

            return a;
        }

        public IEnumerator<T> GetEnumerator()
        {
            return new LargeListEnumerator<T>(this);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return new LargeListEnumerator<T>(this);
        }
    }
}
