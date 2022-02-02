using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace NullFramework.Runtime
{

    /// <summary>
    /// 处理人物行为的虚拟机 (不考虑浮点数)
    /// </summary>
    public partial class ByteCodeBehaviorVM
    {
        /// <summary>
        /// 添加一个值的方法序号
        /// </summary>
        public const int PushValueCode = 1;
        /// <summary>
        /// 添加一组值的方法序号
        /// </summary>
        public const int PushValuesCode = 2;
        /// <summary>
        /// 判断语句 == if
        /// </summary>
        public const int JundgeCode = 3;

        protected static object[] s_param0 = new object[0];
        private static System.Type s_uniTaskType = typeof(UniTask);

        //字节码对应的方法  所有方法的返回类型只会是void 或者 UniTask
        private Dictionary<int, MethodInfo> m_methodDic;
        //当前执行的行为 从中获取所需的环境变量
        //protected Stack<ByteCodeBehavior> m_behaviorStack;
        
        public ByteCodeBehaviorVM()
        {
            m_methodDic = new Dictionary<int, MethodInfo>();
            //m_behaviorStack = new Stack<ByteCodeBehavior>();
            var flags = BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Static;
            var ms = this.GetType().GetMethods(flags);
            for (int i = 0; i < ms.Length; i++)
            {
                var m = ms[i];
                var info = m.GetCustomAttribute<ByteCodeAttribute>(false);
                if(info == null)
                {
                    continue;
                }
            #if UNITY_EDITOR
                if(m_methodDic.ContainsKey(info.Id))
                {
                    Debug.LogError($"方法ID : {info.Id}重复 ");
                }
            #endif
                m_methodDic[info.Id] = m;
            }
        }
        
        
        /// <summary>
        /// 同步执行
        /// </summary>
        public void SyncExecuteBehaviour(ByteCodeBehavior behaviour)
        {
            

            behaviour.Init();
            var param = new object[]{behaviour};
            while (!behaviour.IsEnd())
            {
                var code = behaviour.PopCode();
                var m = m_methodDic[code];
            #if UNITY_EDITOR
                // var info = m.GetCustomAttribute<SkillNodeInfoAttribute>(false);
                // Debug.Log(info.Desc);
            #endif
                //bool isAsync = m.ReturnType == s_uniTaskType;
                m.Invoke(this, param);
            }
            
        }

        //获得一个节点 用于异步执行
        public BehaviorLeaf GetAsyncBehaviorLeaf(ByteCodeBehavior behaviour)
        {
            var leaf = new BehaviorLeaf();

            behaviour.Init();
            var param = new object[]{behaviour};
            leaf.AddBehaviorUpdate(behaviour, ()=>
            {
                behaviour.Start();
                while (!behaviour.IsStop() && !behaviour.IsEnd())
                {
                    var code = behaviour.PopCode();
                    var m = m_methodDic[code];
                    m.Invoke(this, param);
                }
            });
            return leaf;
        }
        //所有方法都是纯函数

        //1-99 用于一些不需要自动生成编辑节点的方法
        [ByteCode(PushValueCode, "字节码的下一个值放入堆栈")]
        private static void PushValue(ByteCodeBehavior behavior)
        {
            var value = behavior.PopCode();
            behavior.PushValue(value);
        }

        //+1 取得值的长度n 
        //+n 将所有值取出放入对象集合中
        [ByteCode(PushValuesCode, "将字节码的一系列值放入堆栈")]
        private void PushValues(ByteCodeBehavior behavior)
        {
            var len = behavior.PopCode();
            var values = behavior.PopCodes(len);
            behavior.PushValues(values); 
        }

        // 1. 分支1字节长度  2.分支2字节长度
        //将两个分支的结束点推入栈中
        [ByteCode(JundgeCode, "if语句")]
        private void JudgeValue(ByteCodeBehavior behavior)
        {
            var isTrue = behavior.PopValue() == 1;
            var len1 = behavior.PopValue();
            var len2 = behavior.PopValue();
            var endPtr = behavior.GetCodePtr() + len1 + len2;
            behavior.PushValue(endPtr);
            if(!isTrue)
            {
                behavior.SetCodePtr(behavior.GetCodePtr() + len1);
            }
        }

        [ByteCode(100, "加法", false, true, "left", "right")]
        private static void Add(ByteCodeBehavior behavior)
        {
            var left = behavior.PopValue();
            var right = behavior.PopValue();
            var result = left + right;
            behavior.PushValue(result);
        }

        [ByteCode(101, "乘法", false, true, "left", "right")]
        private static void Mult(ByteCodeBehavior behavior)
        {
            var left = behavior.PopValue();
            var right = behavior.PopValue();
            var result = left * right;
            behavior.PushValue(result);
        }

        [ByteCode(102, "减法", false, true, "left", "right")]
        private static void Sub(ByteCodeBehavior behavior)
        {
            var left = behavior.PopValue();
            var right = behavior.PopValue();
            var result = left - right;
            behavior.PushValue(result);
        }

        [ByteCode(103, "等待", false, true, "time")]
        private static void Wait(ByteCodeBehavior behavior)
        {
            int key_waitTime = 10;
            var ptr_code = behavior.GetCodePtr() - 1;
            var ptr_value = behavior.GetValuePtr();

            var waitTimer = behavior.GetData(key_waitTime);
            var time = behavior.PopValue();
            //恢复代码和值 并设置等待事件
            if(waitTimer < time)
            {
                behavior.SetData(key_waitTime, waitTimer + 1);
                behavior.SetCodePtr(ptr_code);
                behavior.SetValuePtr(ptr_value);
                behavior.Stop();
                Debug.Log($"wait:{waitTimer}帧");
            }
            //结束时候 清空设置的值
            else
            {
                behavior.RemoveData(key_waitTime);
            }
            
        }
        
    }
}
