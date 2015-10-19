using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Massive.Mathematics
{
    public class VirtualArray<T> : IEnumerable<T>
    {
        private int sub_array_size;

        T[][] data;

        protected VirtualArray()
        {
            data = null;
            sub_array_size = 0;
        }

        public VirtualArray(int sub_length)
            :this(sub_length, 1)
        {
        }

        public VirtualArray(int sub_length, int array_count)
        {
            sub_array_size = sub_length;
            data = new T[array_count][];
            for (int i = 0; i < array_count; i++)
                data[i] = new T[sub_length];
        }

        public VirtualArray(T[] array, int sub_length)
        {
            sub_array_size = sub_length;
            int lng = (array.Length > 0 ? (array.Length - 1) / sub_length + 1 : 0);
            long cIndex = 0;
            data = new T[lng][];

            for (int i = 0; i <lng; i++)
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
                return data[index / sub_array_size][index % sub_array_size];
            }
            set
            {
                data[index / sub_array_size][index % sub_array_size] = value;
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
            if (index >= MaxLength )
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

        public VirtualArray<T> Clone()
        {
            VirtualArray<T> n = new VirtualArray<T>(sub_array_size, data.Length);
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


        public IEnumerator<T> GetEnumerator()
        {
            return new VirtualArrayEnumerator<T>(this);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return new VirtualArrayEnumerator<T>(this);
        }
    }
}
