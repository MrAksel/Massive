using System.Collections;
using System.Collections.Generic;

namespace Massive.Collections
{
    internal class LargeListEnumerator<T> : IEnumerator<T>
    {
        private LargeList<T> virtualArray;

        private long currentIndex;

        public LargeListEnumerator(LargeList<T> virtualArray)
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
