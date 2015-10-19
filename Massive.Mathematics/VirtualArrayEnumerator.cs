using System;
using System.Collections;
using System.Collections.Generic;

namespace Massive.Mathematics
{
    internal class VirtualArrayEnumerator<T> : IEnumerator<T>
    {
        private VirtualArray<T> virtualArray;

        private long currentIndex;

        public VirtualArrayEnumerator(VirtualArray<T> virtualArray)
        {
            this.virtualArray = virtualArray;
        }

        public T Current
        {
            get
            {
                return virtualArray[currentIndex];
            }
        }

        object IEnumerator.Current
        {
            get
            {
                return virtualArray[currentIndex];
            }
        }

        public bool MoveNext()
        {
            currentIndex++;
            return currentIndex < virtualArray.Length;
        }

        public void Reset()
        {
            currentIndex = -1;
        }
        

        public void Dispose()
        {

        }
    }
}