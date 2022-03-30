using System;
using UnityEngine;
namespace NullFramework.Runtime
{
    
    public class RingArray<T>
    {
        protected T[] values;
        protected int start;
        protected int end;
        protected int size;
        protected int count;
        public int Count { get => count; }
       

        public bool IsFull()
        {
            //最大容量为size - 1
            return count == size;
        }
        
        public RingArray(int _size)
        {
            size = _size;
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

        public int Push(T value)
        {
            var pos = end;
            values[end] = value;
            end = (end + 1) % size;
            if(IsFull())
            {
                start = end;
            }
            else
            {
                count ++;
            }
            return end;
        }
        public T Peek()
        {
        #if UNITY_EDITOR
            if(count == 0)
            {
                throw new Exception("当前数量为0 peak无意义");
            }
        #endif
            return values[end];
        }
        public T Head()
        {
        #if UNITY_EDITOR
            if(count == 0)
            {
                throw new Exception("当前数量为0 Head无意义");
            }
        #endif
            return values[start];
        }
        public virtual T Pop()
        {
        #if UNITY_EDITOR
            if(count == 0)
            {
                throw new Exception("当前数量为0 Pop无意义");
            }
        #endif
            var value = values[(end - 1 + size) % size];
            if(count > 0)
            {
                count --;
                end = (end + size - 1) % size;
            }
            return value;
        }

        public virtual T Dequeue()
        {
        #if UNITY_EDITOR
            if(count == 0)
            {
                throw new Exception("当前数量为0 无法Dequeue");
            }
        #endif
            var value = values[start];
            if(count > 0)
            {
                count --;
                start = (start + 1) % size;
            }
            return value;
        }

        public void Clear()
        {
            count = 0;
            start = end = 0;
        }

        public T[] GetArray()
        {
            var res = new T[count];
            for (int i = 0; i < count; i++)
            {
                res[i] = values[(start + i) % size];
            }
            return res;
        }
        public override string ToString()
        {
            var str = "[";
            for (int i = 0; i < count; i++)
            {
                var value = values[(start + i) % size];
                str += i == count - 1 ? $"{value}" : $"{value} ,";
            }
            str += "]";
            return str;
        }
        public T Get(int pos)
        {
        #if UNITY_EDITOR
            if(pos < 0 || pos >= size)
            {
                throw new System.Exception("无效的位置");
            }
            var dis =  (pos + size - start) % size;
            if(dis > count)
            {
                throw new System.Exception("该位置不是存值范围");
            }
        #endif
            return values[pos];
        }
    }

}