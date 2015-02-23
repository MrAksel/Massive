using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Massive.IO.Generic
{
    class FIFOStream<T>
    {
        private LinkedList<T[]> backbuff;

        public FIFOStream()
        {
            backbuff = new LinkedList<T[]>();
        }

        public bool CanRead
        {
            get { return true; }
        }

        public bool CanSeek
        {
            get { return false; }
        }

        public bool CanWrite
        {
            get { return true; }
        }

        public long Length
        {
            get { return backbuff.Sum(b => b.LongLength); }
        }


        public int Read(T[] buffer, int offset, int count)
        {
            if (buffer.Length - offset - count < 0)
                throw new InvalidOperationException();


            int c = 0;
            while (c < count && backbuff.Count > 0)
            {
                T[] item = backbuff.First.Value;
                backbuff.RemoveFirst();

                int copy = Math.Min(item.Length, count - c);
                Array.Copy(item, 0, buffer, c, copy);
                c += copy;

                if (item.Length > copy)
                {
                    T[] rest = new T[item.Length - copy];
                    Array.Copy(item, copy, rest, 0, rest.Length);
                    backbuff.AddFirst(rest);
                }
            }

            return c;
        }

        public void SetLength(long value)
        {
            long lgth = Length;
            if (lgth > value)
            {
                long l = 0;
                LinkedListNode<T[]> node = backbuff.First;
                while (l < value)
                {
                    if (l + node.Value.LongLength > value)
                        break;
                    node = node.Next;
                    l += node.Value.Length;
                }
                T[] buf = node.Value;
                Array.Resize(ref buf, (int)(value - l));
                backbuff.AddBefore(node, buf);

                while (node.Next != null)
                {
                    node = node.Next;
                    backbuff.Remove(node.Previous);
                }
                backbuff.Remove(node);
            }
            else
            {
                T[] buf = new T[value - lgth];
                backbuff.AddLast(buf);
            }
        }

        public void Write(T[] buffer, int offset, int count)
        {
            if (count < 1)
                return;

            T[] buf = new T[count];
            Array.Copy(buffer, offset, buf, 0, count);
            backbuff.AddLast(buf);
        }
    }
}
