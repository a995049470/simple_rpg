﻿using System;
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
        public int Frame { get => m_frame; }
        private int fps = 30;
        public int FPS { get => fps; set => fps = value; }
        //用于帧率计算
        public float FrameDeltaTime { get => 1.0f / fps; }
        private float realDeltaTime;
        public float RealDeltaTime { get => realDeltaTime; }
        private float lastUpdateTime;
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
            InputProcess();
            time += deltaTime;
            while (time > FrameDeltaTime)
            {
                realDeltaTime = Time.time - lastUpdateTime;
                time -= FrameDeltaTime; 
                FrameUpdate();
            }
        }

        //直接帧驱动
        public void FrameUpdate()
        {
            AddFixedMsgs();
            //全执行..
            int count = msgQueue.Count;
            for (int i = 0; i < count; i++)
            {
                var msg = msgQueue.Dequeue();
                InvokeMsg(msg);
            }
            m_frame++;
            //交换缓存
            Swap();
            lastUpdateTime = Time.time;
        }

        private void InvokeMsg(Msg msg)
        {
            if(msg.Sender == null)
            {
                this.OnUpdate(msg);
            }
            else
            {
                msg.Sender.OnUpdate(msg);
            }
        }
        

        private void Swap()
        {
            var temp = msgQueue;
            msgQueue = nextFrameMsgQueue;
            nextFrameMsgQueue = temp;
        }

        /// <summary>
        /// 处理外界的输入
        /// </summary>
        protected virtual void InputProcess()
        {

        }
        /// <summary>
        /// 一些固定的帧事件
        /// </summary>
        protected virtual void AddFixedMsgs()
        {

        }

        //立马执行Msg
        public void SyncExecuteMsg(Msg msg)
        {
            InvokeMsg(msg);
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

