using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameSpace.Core
{
    public class ListIterator<T>
    {
        private int currentIndex = 0;

        private IList<T> source = null;

        public ListIterator(IList<T> source)
        {
            SetSource(source);
        }

        public void SetSource(IList<T> source)
        {
            Reset();
            this.source = source;
        }

        public IList<T> GetSource()
        {
            return source;
        }

        public T GetHead()
        {
            Reset();
            if (source.Count > 0)
            {
                return source[0];
            }
            return default(T);
        }

        public T GetTail()
        {
            return source[GetSize() - 1];
        }

        public int GetSize()
        {
            return source.Count;
        }

        public T GetNext()
        {
            T result = default(T);
            if (HasNext())
            {
                currentIndex++;
                result = source[currentIndex];
            }
            return result;
        }

        public void MoveNext()
        {
            if (HasNext())
                currentIndex++;
        }

        public T GetPrev()
        {
            T result = default(T);
            if (HasPrev())
            {
                currentIndex--;
                result = source[currentIndex];
            }
            return result;
        }

        public void MovePrev()
        {
            if (HasPrev())
                currentIndex--;
        }

        public bool HasNext()
        {
            int nextIndex = currentIndex + 1;
            return nextIndex <= GetSize() - 1;
        }

        public bool HasPrev()
        {
            return currentIndex > 0;
        }

        public bool IsEnd()
        {
            return GetCurrent().Equals(GetTail());
        }

        public bool IsBegin()
        {
            return GetCurrent().Equals(GetHead());
        }

        public T GetCurrent()
        {
            return source[currentIndex];
        }

        public void Reset()
        {
            currentIndex = 0;
        }

        public void SetCurrentIndex(int value)
        {
            currentIndex = value;
        }

        public int GetCurrentIndex()
        {
            return currentIndex;
        }
    }
}
