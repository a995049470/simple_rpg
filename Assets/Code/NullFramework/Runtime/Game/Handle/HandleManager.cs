using System;
using System.Collections.Generic;

namespace NullFramework.Runtime
{
    public class HandleManager :  Single<HandleManager>, IDisposable
    {
        private List<object> m_dataList;
        private List<UInt16> m_magicList;
        private Stack<UInt16> m_freeSoltStack;
        
        public HandleManager()
        {
            m_dataList = new List<object>();
            m_magicList = new List<UInt16>();
            m_freeSoltStack = new Stack<UInt16>();
        }

        public T Get<T>(Handle<T> handle) where T : class
        {
            T value = null;
            var index = handle.GetIndex();
            if(index < m_dataList.Count && 
            m_magicList[index] == handle.GetMagic())
            {
                value = m_dataList[index] as T;
            }
            return value;
        }
        
        public bool IsInvalidHandle(int ptr)
        {
            bool isInvalid = Get<object>(ptr) == null;
            return isInvalid;
        }

        public bool IsInvalidHandle<T>(Handle<T> handle) where T : class
        {
            bool isInvalid = Get(handle) == null;
            return isInvalid;
        }

        public T Get<T>(int ptr) where T : class
        {
            Handle<T> handle = new Handle<T>(ptr);
            T value = Get(handle);
            return value;
        }

        

        public Handle<T> Put<T>(T value) where T : class
        {
            Handle<T> handle = default;
            if(m_freeSoltStack.Count > 0)
            {
                UInt16 index = m_freeSoltStack.Pop();
                handle = new Handle<T>(index);
                m_magicList[index] = handle.GetMagic();
                m_dataList[index] = value;
            }
            else
            {
                UInt16 index = (UInt16)m_dataList.Count;
                handle = new Handle<T>(index);
                m_magicList.Add(handle.GetMagic());
                m_dataList.Add(value);
            }
            if(value is IHandle iHandle)
            {
                iHandle.SetHandle(handle.GetHandle_I32());
            }
            return handle;
        }

        public Int32 Put_I32<T>(T value) where T : class
        {
            return Put(value).GetHandle_I32();
        }

        public bool TryRePut<T>(Handle<T> handle, out Handle<T> reHandle) where T : class
        {
            bool isRePut = false;
            T value = Get(handle);
            reHandle = new Handle<T>(0);
            if(value != null)
            {
                isRePut = true;
                Free(handle);
                reHandle = Put(value);
            }
            return isRePut;
        }

        public bool TryRePut<T>(int ptr, out int rePtr) where T : class
        {
            bool isRePut = false;
            rePtr = 0;
            T value = Get<T>(ptr);
            if(value != null)
            {
                isRePut = true;
                Free(ptr);
                rePtr = Put_I32(value);
            }
            return isRePut;
        }
        
        /// <summary>
        /// 释放变量的唯一引用(临时变量一定要在域内的时候free)
        /// </summary>
        /// <param name="handle"></param>
        public void Free<T>(Handle<T> handle) where T : class
        {
            var index = handle.GetIndex();
            if(index < m_dataList.Count &&
            m_magicList[index] == handle.GetMagic())
            {
                m_magicList[index] = 0;
                var value = m_dataList[index];
                if(value is IHandle iHandle)
                {
                    iHandle.OnFree();
                }
                m_dataList[index] = null;
                m_freeSoltStack.Push(index);
            }
        }

        /// <summary>
        /// 释放变量的唯一引用(临时变量一定要在域内的时候free)
        /// </summary>
        /// <param name="handle"></param>
        public void Free(int handle)
        {
            Free(new Handle<object>(handle));
        }

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override string ToString()
        {
            return base.ToString();
        }

        public void Dispose()
        {
            
        }
    }
    
}

