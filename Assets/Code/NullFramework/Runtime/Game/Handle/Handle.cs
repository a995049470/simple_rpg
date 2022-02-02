
using System;
using UnityEngine;
namespace NullFramework.Runtime
{
    
    public struct Handle<T> where T : class
    {
        private UInt16 m_index;
        private UInt16 m_magic;
        private static UInt16 s_maxMagic = (1 << 16) - 1;
        private static UInt16 s_autoMagic = 0;

        public Handle(UInt16 index)
        {
            if(++s_autoMagic > s_maxMagic)
            {
                s_autoMagic = 1; // 0表示空句柄
            }
            m_index = index;
            m_magic = s_autoMagic;
        }

        public Handle(UInt32 value)
        {
            m_index = (UInt16)(value >> 16);
            m_magic = (UInt16)value;
        }

        public Handle(int value)
        {
            m_index = (UInt16)(value >> 16);
            m_magic = (UInt16)value;
        }

        public UInt16 GetIndex() { return m_index; }
        public UInt16 GetMagic() { return m_magic; } 
        public UInt32 GetHandle_U32() { return ((UInt32)m_index << 16) | (UInt32)m_magic; }
        public Int32 GetHandle_I32() { return ((Int32)m_index << 16) | (Int32)m_magic; }
        public T Get()
        {
            return HandleManager.Instance.Get(this);
        }

        public Handle<U> Convert<U>() where U : class
        {
            return new Handle<U>(GetHandle_I32());
        }

        public void Free()
        {
            HandleManager.Instance.Free(this);   
            m_magic = 0;//表示为空
        }

        public bool IsInvalid()
        {
            return HandleManager.Instance.IsInvalidHandle(this);
        }

        public static bool operator == (Handle<T> left, Handle<T> right)
        {
            return left.GetHandle_I32() == right.GetHandle_I32();
        }

        public static bool operator != (Handle<T> left, Handle<T> right)
        {
            return left.GetHandle_I32() != right.GetHandle_I32();
        }
    }
    
}
