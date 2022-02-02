using System;
using UnityEngine;
namespace NullFramework.Runtime
{
    //考虑动态扩容的问题 实际容量会比数组尺寸少一个
    public class RingArray<T>
    {
        private T[] values;
        private int start;
        private int end;
        private int size;
        public int ValueCount { get => (end + size - start) % size;}
       

        public bool IsFull()
        {
            //最大容量为size - 1
            return ValueCount == size - 1;
        }
        
        public RingArray(int _size)
        {
            size = _size + 1;
            values = new T[size];
        }

        public int GetEndPtr()
        {
            return end;
        }

        public void SetEndPtr(int ptr)
        {
            end = ptr;
        }

        public int GetStartPtr()
        {
            return start;
        }

        public void SetStartPtr(int ptr)
        {
            start = ptr;
        }

        public void Push(T value)
        {
            values[end] = value;
            end = (end + 1) % size;
            if(end == start)
            {
                start = (start + 1) % size;
            }
        }
        public T Peek()
        {
            return values[end];
        }
        public T Head()
        {
            return values[start];
        }
        public T Pop()
        {
            var value = values[(end - 1 + size) % size];
            if(end != start)
            {
                end = (end + size - 1) % size;
            }
            return value;
        }

        public T Dequeue()
        {
            var value = values[start];
            if(start != end)
            {
                start = (start + 1) % size;
            }
            return value;
        }

        public void Clear()
        {
            start = end = 0;
        }

        public T[] GetArray()
        {
            int count = (end + size - start) % size;
            var res = new T[count];
            for (int i = 0; i < count; i++)
            {
                res[i] = values[(start + i) % size];
            }
            return res;
        }
    }

}