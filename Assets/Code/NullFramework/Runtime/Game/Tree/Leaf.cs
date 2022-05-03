using System;
using System.Collections.Generic;

namespace NullFramework.Runtime
{
    public abstract class Leaf<T> : Leaf, ILeafDataReciver where T : LeafData
    {
        protected T leafData;

        public virtual void OnReciveDataFinish() {}

        public virtual void SetLeafData(LeafData data)
        {
            this.leafData = data as T;
        }
    }
    
    //实际生效的叶子节点
    public abstract class Leaf 
    {
        protected static Root root { get => Root.Instance; }

        protected Tress parent;
        protected Dictionary<int, MsgRespond> m_msgRespondMap;
        //叶节点种类
        private int kind;
        public int Kind { get => kind; }
        private bool isActive = false;
        public bool IsActive { get => isActive; }
        private bool isWake = false;
        private bool m_isRegisterMsgHandle = false;
        //深度
        private int depth;
        public int Depth { get => depth; }
        public delegate Action MsgListener(Msg msg); 
        protected static Action emptyAction = null;
        //TODO:计划使用uid 只要被free 原来的uid失效 思考中
        // private long uid;
        // public long UID { get => uid; }


        //同一种kind 不同序号
        //private int m_index;
        public Leaf()
        {
            m_msgRespondMap = new Dictionary<int, MsgRespond>();
           
        }
        
        public virtual void LoadData(LeafData data)
        {
            if(this is ILeafDataReciver setter)
            {
                setter.SetLeafData(data);
                setter.OnReciveDataFinish();
            }
        }

        //激活或进入运行栈
        public virtual void OnEnable(Leaf lastLeaf = null)
        {
            isWake = true;
            isActive = true;
        }

        
        public void SetActive(bool _active)
        {
            if(isActive == _active) return;
            isActive = _active;
            if(isActive) OnEnable();
            else OnDisable();
        }

        protected virtual void InitListeners() {}

        /// <summary>
        /// 在激活的状态下是否存在回应
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        public virtual bool OnUpdate(Msg msg)
        {
            bool isHasRespond = true;
            //失活态不除发OnUpdate
            if(!isActive || !isWake)
            {
                return isHasRespond;
            }

            //现在是深度优先的遍历方式

            if(m_msgRespondMap.TryGetValue(msg.Kind, out var respond))
            {
                // UnityEngine.Debug.Log($"{this.GetType()} 接收导了 {msg.GetData().GetType()}");
                respond.Invoke(msg);
                isHasRespond = !respond.IsEmpty();
            }
            else
            {
                isHasRespond = false;
            }
            return isHasRespond;
        }

        //失去活力或退出运行栈时
        public virtual void OnDisable(Leaf nextHandle = default)
        {   
            isWake = false;
            isActive = false;
        }
        
       

        //状态机节点休眠 为下弹式状态机设计
        public void Sleep()
        {
            isWake = false;
        }

        //状态机节点苏醒 为下弹式状态机设计
        public void Wake()
        {
            isWake = true;
        }

        

        public void SetLeafKind(int _kind)
        {
            this.kind = _kind;
        }


       
        //被释放时触发
        public virtual void OnFree()
        {

        }

        public void AddMsgListener(int msgKind, MsgListener action)
        {
            if (m_msgRespondMap.TryGetValue(msgKind, out var result_respond))
            {
                result_respond.AddMsgAction(action);
            }
            else
            {
                var msgRespond = new MsgRespond();
                msgRespond.AddMsgAction(action);
                m_msgRespondMap[msgKind] = msgRespond;
                parent?.AddMsgLeaf(msgKind, this);
            }
        }

        public void AddMsgListeners(params (int, MsgListener)[] listeners)
        {
            foreach (var listener in listeners)
            {
                AddMsgListener(listener.Item1, listener.Item2);
            }
        }

        public void RemoveMsgListener(int msgKind, MsgListener action)
        {
            if (m_msgRespondMap.TryGetValue(msgKind, out var respond))
            {
                respond.RemoveMsgAction(action);
                if (respond.IsEmpty())
                {
                    m_msgRespondMap.Remove(msgKind);  
                }
            }
        }

        public void ClearMsgListeners()
        {
            foreach (var key in m_msgRespondMap.Keys)
            {
                parent.RemoveMsgLeaf(key, this);
            }
            m_msgRespondMap.Clear();
        }

        //成为信息流动节点
        private void RegisterMsgForParent()
        {
            if(m_isRegisterMsgHandle) return;
            var parent = this.parent;
            if (parent != null)
            {
                m_isRegisterMsgHandle = true;
                foreach (var key in m_msgRespondMap.Keys)
                {
                    parent.AddMsgLeaf(key, this);
                }

            }
        }

        //TODO:考虑切换父物体的问题
        //考虑切换父物体是 改变handle
        public void SetParent(Tress _parent, bool isActive = true)
        {
            if (parent == _parent)
            {
                return;
            }  
            if(parent != null)
            {
                ClearMsgListeners();
            }
            parent = _parent;
            depth = parent.Depth + 1;
            InitListeners();
            //传输数据
            // if(this is ILeafMemberDicSetter setter && parent is ILeafMemberDicGetter getter)
            // {
            //     setter.SetMemberDic(getter.GetMemberDic());
            // }
            parent.AddChild(this, isActive);
        }  

    }
}

