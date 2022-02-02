using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace NullFramework.Runtime
{
    //所有节点的根节点 暂时只会有一个
    public class Root : Tress
    {
        
        private static Root m_instance;
        public static Root Instance
        {
            get
            {
                if(m_instance == null)
                {
                    m_instance = new Root();
                }
                return m_instance;
            }
        }
        private const int msgSize = 256;
        private RingArray<Msg> msgRingArray;
        private int m_frame;
        private int Frame { get => m_frame; }
        
        public Root() : base()
        {
            msgRingArray = new RingArray<Msg>(msgSize);
            m_frame = 0;
            OnEnter();
        }

        //用于外界调用的Update
        public void Update()
        {
            var msgs = msgRingArray.GetArray();
            msgRingArray.Clear();
            foreach (var msg in msgs)
            {
                OnUpdate(msg);
            }
            m_frame ++;
            
        }

        //立马执行Msg
        public void SyncExecuteMsg(Msg msg)
        {
            OnUpdate(msg);
        }


        //最大接受256条 之后的会被丢弃  之后在考虑丢弃问题..
        public void AddMsg(Msg msg)
        {
        #if UNITY_EDITOR
            if(msgRingArray.IsFull())
            {
                Debug.LogError("命令列表满了....考虑扩容或者换个存储列表...");
            }
        #endif
            msgRingArray.Push(msg);
        }        
    }
}

