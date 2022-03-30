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
        //超过最大层数 如果消息还没激活 就会停止传播
        private int maxDepth;
        private bool isActive;
        private bool isStop;
        //是否停止传播
        public bool IsStop { get => isStop; }

        public Msg(int _kind, object _data = null, Leaf _sender = null, Leaf _reciver = null)
        {
            this.kind = _kind;
            this.data = _data;
            this.sender = _sender;
            this.reciver = _reciver;
            this.isActive = _reciver == null;
            this.isStop = false;
            this.maxDepth = _reciver == null ? int.MaxValue : _reciver.Depth;
        }

        public bool ActiveMsg(Leaf leaf)
        {
            if(!isActive)
            {
                isActive = leaf == reciver;
                if (!isActive) isStop = leaf.Depth >= maxDepth;
            }
            return isActive;
        }


        

        public T GetData<T>() where T : class
        {
            return data as T;
        }

        public object GetData()
        {
            return data;
        }
       
    }
}
