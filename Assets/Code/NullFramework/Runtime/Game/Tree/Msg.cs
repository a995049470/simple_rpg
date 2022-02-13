using System;

namespace NullFramework.Runtime
{
    public class Msg
    {
        public int Kind;
        private object m_data;
        //传送者
        public int Sender;
        //Msg的信息更新从接收者开始
        public int Reciver;
        public int Count;
        //是否输入(需要记录)
        public bool IsInput;
        public int Frame;

        public Msg(int kind, object data = null, int sender = 0, int reciver = 0)
        {
            this.Kind = kind;
            this.m_data = data;
            this.Sender = sender;
            this.Reciver = reciver;
        }

        public T GetData<T>() where T : class
        {
            return m_data as T;
        }

        public object GetData()
        {
            return m_data;
        }
       
    }
}
