using System;
using System.Collections.Generic;
using System.Text;

namespace Rawr.Mage
{
    public enum HeapType
    {
        MinimumHeap,
        MaximumHeap
    }

    public class Heap<T> where T : IComparable<T>
    {
        private int cap;
        private T[] list;
        private int mCount;
        private bool typeLogic;

        public Heap(HeapType type)
        {
            this.cap = 16;
            this.list = new T[this.cap + 1];
            if (type == HeapType.MaximumHeap)
            {
                this.typeLogic = true;
            }
        }

        public Heap(int capacity, HeapType type)
        {
            this.cap = capacity;
            this.list = new T[this.cap + 1];
            if (type == HeapType.MaximumHeap)
            {
                this.typeLogic = true;
            }
        }

        public void Clear()
        {
            this.mCount = 0;
        }

        public T Pop()
        {
            if (this.mCount == 0)
            {
                return default(T);
            }
            T entry = this.list[1];
            int mCount = this.mCount;
            int index = mCount;
            mCount--;
            int num3 = 1;
            while ((num3 * 2) <= mCount)
            {
                int num = num3 * 2;
                if (((num + 1) <= mCount) && ((((IComparable<T>)this.list[num + 1]).CompareTo(this.list[num]) < 0) ^ this.typeLogic))
                {
                    num++;
                }
                if (!((((IComparable<T>)this.list[index]).CompareTo(this.list[num]) > 0) ^ this.typeLogic))
                {
                    break;
                }
                this.list[num3] = this.list[num];
                num3 = num;
            }
            this.list[num3] = this.list[index];
            this.mCount--;
            return entry;
        }

        public void Push(T entry)
        {
            if (this.mCount == cap)
            {
                cap *= 2;
                T[] newList = new T[cap + 1];
                Array.Copy(list, newList, list.Length);
                list = newList;
            }
            this.mCount++;
            int mCount = this.mCount;
            while ((mCount > 1) && ((((IComparable<T>)this.list[mCount / 2]).CompareTo(entry) > 0) ^ this.typeLogic))
            {
                this.list[mCount] = this.list[mCount / 2];
                mCount = mCount / 2;
            }
            this.list[mCount] = entry;
        }

        public int Count
        {
            get
            {
                return this.mCount;
            }
        }

        public T Head
        {
            get
            {
                if (this.mCount == 0)
                {
                    return default(T);
                }
                return this.list[1];
            }
        }

        public T this[int index]
        {
            get
            {
                return this.list[index];
            }
        }
    }
}
