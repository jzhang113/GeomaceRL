using System;
using System.Collections;
using System.Collections.Generic;

namespace GeomaceRL
{
    public class MaxHeap<T> : ICollection<T> where T : IComparable<T>
    {
        private T[] _heap;

        public int Count { get; private set; }
        public bool IsReadOnly => false;

        public MaxHeap(int size)
        {
            _heap = new T[size];
            Count = 0;
        }

        public T Peek()
        {
            if (Count > 0)
                return _heap[0];

            throw new InvalidOperationException("The heap is empty");
        }

        public T PopMax()
        {
            if (Count <= 0)
                throw new InvalidOperationException("The heap is empty");

            T item = _heap[0];
            --Count;
            _heap[0] = _heap[Count];
            ReheapDown(0);
            return item;
        }

        public void Clear()
        {
            Count = 0;
        }

        public void Add(T item)
        {
            if (Count >= _heap.Length)
                Resize();

            _heap[Count] = item;
            Count++;
            ReheapUp(Count - 1);
        }

        public bool Contains(T item)
        {
            for (int i = 0; i < Count; i++)
            {
                if (_heap[i].Equals(item))
                    return true;
            }
            return false;
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            _heap.CopyTo(array, arrayIndex);
        }

        public bool Remove(T item)
        {
            for (int i = 0; i < Count; i++)
            {
                if (_heap[i].Equals(item))
                {
                    T oldItem = _heap[i];

                    --Count;
                    T newItem = _heap[Count];
                    _heap[i] = _heap[Count];

                    if (CompareItem(oldItem, newItem) > 0)
                        ReheapUp(i);
                    else
                        ReheapDown(i);
                    return true;
                }
            }
            return false;
        }

        public IEnumerator<T> GetEnumerator()
        {
            for (int i = 0; i < Count; i++)
            {
                yield return _heap[i];
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private void ReheapUp(int initial)
        {
            int pos = initial;
            T oldItem = _heap[pos];
            T newItem = _heap[(pos - 1) / 2];

            while (pos > 0 && CompareItem(oldItem, newItem) > 0)
            {
                _heap[pos] = newItem;

                pos = (pos - 1) / 2;
                newItem = _heap[(pos - 1) / 2];
            }

            _heap[pos] = oldItem;
        }

        private void ReheapDown(int initial)
        {
            int pos = initial;
            T oldItem = _heap[pos];

            while (pos < Count)
            {
                int left = 2 * pos + 1;
                int right = 2 * pos + 2;
                int swap = pos;

                if (left < Count && CompareItem(_heap[swap], _heap[left]) < 0)
                    swap = left;

                if (right < Count && CompareItem(_heap[swap], _heap[right]) < 0)
                    swap = right;

                if (swap == pos)
                {
                    break;
                }
                else
                {
                    _heap[pos] = _heap[swap];
                    pos = swap;
                }
            }

            _heap[pos] = oldItem;
        }

        private void Resize()
        {
            T[] newHeap = new T[_heap.Length * 3/2];

            for (int i = 0; i < Count; i++)
            {
                newHeap[i] = _heap[i];
            }

            _heap = newHeap;
        }

        private static int CompareItem(T a, T b)
        {
            return a.CompareTo(b);
        }
    }
}
