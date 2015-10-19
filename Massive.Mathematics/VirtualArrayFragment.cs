using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Massive.Mathematics
{
    class VirtualArrayFragment<T> : VirtualArray<T>
    {
        long offset;
        long length;
        VirtualArray<T> data;
        
        public new long Length
        {
            get
            {
                return length;
            }
        }

        public new T this[long index]
        {
            get
            {
                if (index >= length || index < 0)
                    throw new ArgumentOutOfRangeException("Index must be between 0 and length.");
                if (index + offset >= data.Length)
                    throw new ArgumentOutOfRangeException("Index exceeded base array bounds.");

                return data[index + offset];
            }
            set
            {
                if (index >= length || index < 0)
                    throw new ArgumentOutOfRangeException("Index must be between 0 and length.");
                if (index + offset >= data.Length)
                    throw new ArgumentOutOfRangeException("Index exceeded base array bounds.");

                data[index + offset] = value;
            }
        }

        public VirtualArrayFragment(VirtualArray<T> underlying, long index, long length) 
        {
            this.offset = index;
            this.length = length;
            data = underlying;
        }

        public void SetLength(long length)
        {
            this.length = length;
        }

        public void SetOffset(long offset)
        {
            this.offset = offset;
        }

        public void MoveLength(long lengthChange)
        {
            this.length += lengthChange;
        }

        public void MoveOffset(long offsetChange)
        {
            this.offset += offsetChange;
        }
    }
}
