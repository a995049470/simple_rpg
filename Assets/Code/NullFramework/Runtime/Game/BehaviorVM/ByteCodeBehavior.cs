using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace NullFramework.Runtime
{
    
    ///<summary>用字节码描述一个行为</summary>
    public class ByteCodeBehavior
    {
        
        private const int max_size = 512;
        ///<summary>行为字节码</summary>
        private int[] m_codes;
        //private CodeType m_codeType;
        //字节码指针
        private int m_ptr;
        //对象集合
        private RingArray<int> m_valueStack;
        //用以描述对象集合的连续性
        private RingArray<int> m_continuityStack;
        //是否成功运行
        //如果这是一个条件判定行为 只要该字段为false 就结束判定行为
        private bool m_isSuccess;
        //外界传输的数据字典 key:类型 value: int值 或者句柄
        //相当于堆 key相当变量名
        private Dictionary<int, int> m_dataHeap;
        //是否中止 用于实现异步
        private bool m_isStop = false;
        
        public ByteCodeBehavior(int[] codes) 
        {
            m_ptr = 0;
            m_codes = codes ?? new int[0];
            m_valueStack = new RingArray<int>(max_size);
            m_continuityStack = new RingArray<int>(max_size);
            m_dataHeap = new Dictionary<int, int>();
        }

        public ByteCodeBehavior()
        {
            m_ptr = 0;
            m_codes = new int[0];
            m_valueStack = new RingArray<int>(max_size);
            m_continuityStack = new RingArray<int>(max_size);
            m_dataHeap = new Dictionary<int, int>();
        }

        public void Stop()
        {
            m_isStop = true;
        }    

        public void Start()
        {
            m_isStop = false;
        }

        public bool IsStop()
        {
            return m_isStop;
        }

        /// <summary>
        /// 向外源数据字典中增加值
        /// </summary>
        /// <param name="type"></param>
        /// <param name="handle"></param>
        public void SetData(int type, int handle)
        {
            m_dataHeap[type] = handle;
        }        

        /// <summary>
        /// 获取数据
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public int GetData(int key)
        {
            if(!m_dataHeap.TryGetValue(key,out int value))
            {
                value = 0;
            }
            return value;
        }

        public void RemoveData(int key)
        {
            m_dataHeap.Remove(key);
        }
        /// <summary>
        /// 获取数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public T GetData<T>(int key) where T : class
        {
            if(!m_dataHeap.TryGetValue(key,out int value))
            {
                value = 0;
            }
            return HandleManager.Instance.Get<T>(value);
        }
        

        /// <summary>
        /// 虚拟机开始执行之前的初始化
        /// </summary>
        public void Init()
        {
            m_ptr = 0;
            m_isSuccess = true;
            m_valueStack.Clear();
            m_continuityStack.Clear();
            m_isStop = false;
        }

        public async UniTask AsynceExecute()
        {
           throw new System.Exception("");
        }
        
        public void SyncExecute()
        {
            
        }

        /// <summary>
        /// 获取当前执行结果 是否成功
        /// </summary>
        public bool GetExcuteResult()
        {
            return m_isSuccess;
        }

        /// <summary>
        /// 执行失败
        /// </summary>
        public void SetExecuteFail()
        {
            m_isSuccess = false;
        }

        /// <summary>
        /// 将指针移到最后一位
        /// </summary>
        public void MovePtrToEnd()
        {
            m_ptr = m_codes.Length;
        }
        
        /// <summary>
        /// 改变行为对应的字节码
        /// </summary>
        public void SetCodes(int[] codes)
        {
            m_codes = codes ?? new int[0];
        }

       
    
        ///<summary> 获取字节码</summary>
        public int[] GetCodes()
        {
            return m_codes;
        }

        ///<summary> 弹出一个code, ptr += 1</summary>
        public int PopCode()
        {
            return m_codes[m_ptr++];
        }
        ///<summary> 获取一段code, ptr += count</summary>
        public int[] PopCodes(int count)
        {
            var vlaues = new int[count];
            for (int i = 0; i < count; i++)
            {
                vlaues[i] = m_codes[m_ptr++];
            }
            return vlaues;
        }

        public bool IsEnd()
        {
            return m_ptr >= m_codes.Length || !m_isSuccess;
        }

         ///<summary>向值栈顶放入一个数字</summary>
        public void PushValue(int value)
        {
            m_valueStack.Push(value);
            m_continuityStack.Push(1);
        }


        ///<summary>向值栈顶放入一段数字 </summary>
        public void PushValues(int[] values)
        {
            int cnt = values.Length;
            for (int i = cnt - 1; i >= 0; i--)
            {
                m_valueStack.Push(values[i]);
            }
            m_continuityStack.Push(cnt);
        }
        ///<summary>向值栈顶放入一段数字</summary>
        public void PushValues(List<int> values)
        {
            int cnt = values.Count;
            for (int i = cnt - 1; i >= 0; i--)
            {
                m_valueStack.Push(values[i]);
            }
            m_continuityStack.Push(cnt);
        }

        ///<summary>从值栈顶端取出一个值</summary>
        public int PopValue()
        {

            int cnt = m_continuityStack.Pop();
#if UNITY_EDITOR
            if(cnt != 1)
            {
                Debug.LogError("栈顶连续的数字数量不为1");
            }
#endif
            return m_valueStack.Pop();
        }

        ///<summary>从值栈顶端取出一段值 (倒着取)</summary>
        public int[] PopValues()
        {
            int cnt = m_continuityStack.Pop();
            int[] res = new int[cnt];
            for (int i = 0; i < cnt; i++)
            {
                res[i] = m_valueStack.Pop();
            }
            return res;
        }
        
        /// <summary>
        /// 获取当前代码指针的位置
        /// </summary>
        /// <returns></returns>
        public int GetCodePtr()
        {
            return m_ptr;
        }

        /// <summary>
        /// 设置当前代码指针的位置
        /// </summary>
        public void SetCodePtr(int ptr)
        {
            m_ptr = ptr;
        }
        /// <summary>
        /// 获取当前值栈指针的位置
        /// </summary>
        /// <returns></returns>
        public int[] GetValuePtr()
        {
            return new int[2] {m_valueStack.GetEndPtr(), m_continuityStack.GetEndPtr()};
        }
        /// <summary>
        /// 设置当前值栈指针的位置
        /// </summary>
        /// <param name="valuePtr"></param>
        public void SetValuePtr(int[] valuePtr)
        {
            m_valueStack.SetEndPtr(valuePtr[0]);
            m_continuityStack.SetEndPtr(valuePtr[1]);
        }
    }
}


