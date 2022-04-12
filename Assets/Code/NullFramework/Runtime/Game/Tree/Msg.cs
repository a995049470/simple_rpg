using System;

namespace NullFramework.Runtime
{
    public abstract class Cmd
    {
        //执行
        public abstract void Do();
        //撤销
        public abstract void UnDo();
    }

    public class ValueChangeMsg<T> : Cmd
    {
        private T arg;
        public ValueChangeMsg(ref T arg, T originValue, T currentValue)
        {
            this.arg = arg;
        }
        public override void Do()
        {
            throw new NotImplementedException();
        }

        public override void UnDo()
        {
            throw new NotImplementedException();
        }
    }

    public class Msg
    {
        private int kind;
        public int Kind { get => kind; }
        private object data;
        //传送者
        private Leaf sender;
        //Msg的信息更新从接收者开始
        private Leaf reciver;
        public Leaf Reciver { get => reciver; }
        public Msg(int _kind, object _data = null, Leaf _sender = null, Leaf _reciver = null)
        {
            this.kind = _kind;
            this.data = _data;
            this.sender = _sender;
            this.reciver = _reciver;
        }

        public T GetData<T>() where T : MsgData
        {
            return data as T;
        }

        public object GetData()
        {
            return data;
        }   
    }
}
