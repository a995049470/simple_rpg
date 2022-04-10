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
        
        private static Root instance;
        public static Root Instance
        {
            get
            {
                if(instance == null)
                {
                    instance = new Root();
                }
                return instance;
            }
        }
        private const int MaxMsgCount = 32;
        private Queue<Msg> msgQueue;
        private Queue<Msg> nextFrameMsgQueue;
        private int m_frame;
        private int Frame { get => m_frame; }
        private int fps = 30;
        public int FPS { get => fps; set => fps = value; }
        public float DeltaTime { get => 1.0f / fps; }
        private float time = 0;
        
        public static void Dispose()
        {
            instance = null;
        }

        public Root() : base()
        {
            instance = this;
            msgQueue = new Queue<Msg>();
            nextFrameMsgQueue = new Queue<Msg>();
            m_frame = 0;
            OnEnable();
        }

        //用于外界调用的Update
        public void Update(float deltaTime)
        {
            time += deltaTime;
            while (time > DeltaTime)
            {
                time -= DeltaTime; 
                Update();
            }
        }

        //直接帧驱动
        public void Update()
        {
            BeforeUpdate();
            //全执行..
            int count = msgQueue.Count;
            for (int i = 0; i < count; i++)
            {
                var msg = msgQueue.Dequeue();
                OnUpdate(msg);
            }
            m_frame++;
            //交换缓存
            Swap();
        }


        private void Swap()
        {
            var temp = msgQueue;
            msgQueue = nextFrameMsgQueue;
            nextFrameMsgQueue = temp;
        }

        protected virtual void BeforeUpdate()
        {

        }

        //立马执行Msg
        public void SyncExecuteMsg(Msg msg)
        {
            OnUpdate(msg);
        }


        
        public void AddMsg(Msg msg)
        {
            msgQueue.Enqueue(msg);
        }        

        public void AddNextFrameMsg(Msg msg)
        {
            nextFrameMsgQueue.Enqueue(msg);
        }

        
    }
}

