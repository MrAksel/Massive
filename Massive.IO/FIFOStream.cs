using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Massive.IO
{
    public class FIFOStream : Stream
    {
        private object _locker;

        private LinkedList<byte[]> backbuff;

        public FIFOStream()
        {
            _locker = new object();
            backbuff = new LinkedList<byte[]>();
        }

        public override bool CanRead
        {
            get { return true; }
        }

        public override bool CanSeek
        {
            get { return false; }
        }

        public override bool CanWrite
        {
            get { return true; }
        }

        public override void Flush()
        {

        }

        public override long Length
        {
            get
            {
                lock (_locker) return backbuff.Sum(b => b.LongLength);
            }
        }

        public override long Position
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            if (buffer.Length - offset - count < 0)
                throw new InvalidOperationException();


            int c = 0;
            lock (_locker)
            {
                while (c < count && backbuff.Count > 0)
                {
                    byte[] item = backbuff.First.Value;
                    backbuff.RemoveFirst();

                    int copy = Math.Min(item.Length, count - c);
                    Array.Copy(item, 0, buffer, c, copy);
                    c += copy;

                    if (item.Length > copy)
                    {
                        byte[] rest = new byte[item.Length - copy];
                        Array.Copy(item, copy, rest, 0, rest.Length);
                        backbuff.AddFirst(rest);
                    }
                }
            }

            return c;
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotImplementedException();
        }

        public override void SetLength(long value)
        {
            lock (_locker)
            {
                long lgth = Length;
                if (lgth > value)
                {
                    long l = 0;
                    LinkedListNode<byte[]> node = backbuff.First;
                    while (l < value)
                    {
                        if (l + node.Value.LongLength > value)
                            break;
                        node = node.Next;
                        l += node.Value.Length;
                    }
                    byte[] buf = node.Value;
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
                    byte[] buf = new byte[value - lgth];
                    backbuff.AddLast(buf);
                }
            }
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            if (count < 1)
                return;

            byte[] buf = new byte[count];
            Array.Copy(buffer, offset, buf, 0, count);
            lock (_locker)
                backbuff.AddLast(buf);
        }
    }
}
