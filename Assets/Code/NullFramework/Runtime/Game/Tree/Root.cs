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
        private Queue<Msg> msgQueue;
        private int m_frame;
        private int Frame { get => m_frame; }
        private int fps = 30;
        public int FPS { get => fps; set => fps = value; }
        public float DeltaTime { get => 1.0f / fps; }
        private float time = 0;
        
        
        public Root() : base()
        {
            m_instance = this;
            msgQueue = new Queue<Msg>();
            m_frame = 0;
            OnEnter();
        }

        //用于外界调用的Update
        public void Update(float deltaTime)
        {
            time += deltaTime;
            while (time > DeltaTime)
            {
                time -= DeltaTime; 
                
                BeforeUpdate();
                while(msgQueue.Count > 0)
                {
                    var msg = msgQueue.Dequeue();
                    OnUpdate(msg);
                }
                m_frame ++;
            }
        }


        protected virtual void BeforeUpdate()
        {

        }

        //立马执行Msg
        public void SyncExecuteMsg(Msg msg)
        {
            OnUpdate(msg);
        }


        //最大接受256条 之后的会被丢弃  之后在考虑丢弃问题..
        public void AddMsg(Msg msg)
        {
            msgQueue.Enqueue(msg);
        }        
    }
}

