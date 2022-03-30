using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace NullFramework.Runtime
{

    public class VM
    {
        //代码的二进制文件
        private byte[] codeBytes;
        private int codePtr;
        private RingArray<int> numStack;
        // ObjectArray 暂时难以扩容
        private ObjectArray objArray;
        private const int maxSize_numStack = 256;
        private const int maxSize_objArray = 256;
        protected Dictionary<string, byte> symbolDic; 
        protected Dictionary<byte, MethodInfo> methodDic;
        private bool isStop;
        private bool isFinish;
        private static object[] arg0 = new object[0];

        public VM()
        {
            numStack = new RingArray<int>(maxSize_numStack);
            objArray = new ObjectArray(maxSize_objArray);
            InitSymbolDic();
        }

        public VM(VM vm)
        {
            numStack = new RingArray<int>(maxSize_numStack);
            objArray = new ObjectArray(maxSize_objArray);
            this.symbolDic = vm.symbolDic;
            this.methodDic = vm.methodDic;
        }

        public void Restart()
        {
            numStack.Clear();
            objArray.Clear();
            isStop = false;
            isFinish = false;
            codePtr = 0;
        }

        public bool Excute()
        {
        #if UNITY_EDITOR
            if(codeBytes == null || codeBytes.Length == 0)
            {
                throw new System.Exception("没代码 跑不了...");
            }
        #endif
            isStop = false;  
            while (!isFinish && !isStop)
            {
                var b = PopByteFromeCodeBytes();
                var method = methodDic[b];
                method.Invoke(this, arg0);
                isFinish = codePtr >= codeBytes.Length;
            } 
            return isFinish;
        }

        //初始化符号表
        protected void InitSymbolDic()
        {
            var type = this.GetType();
            var flags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
            var methods = type.GetMethods(flags);
            var vmMethods = new List<MethodInfo>();
            foreach (var method in methods)
            {  
                if(method.GetCustomAttribute(typeof(VMMethodAttribute), false) != null)
                {
                    vmMethods.Add(method);
                }
            }
            vmMethods.Sort((x, y)=>
            {
                return x.Name.ToLower().GetHashCode() - y.Name.ToLower().GetHashCode();
            });
            int count = vmMethods.Count;
            symbolDic = new Dictionary<string, byte>();
            methodDic = new Dictionary<byte, MethodInfo>();
            if(count > 256)
            {
                throw new System.Exception("方法数量大于256 需要改用short");
            }
            for (int i = 0; i < count; i++)
            {
                var method = vmMethods[i];
                var id = (byte)i;
                symbolDic[method.Name.ToLower()] = id;
                methodDic[id] = method;
            }
            
        }

        public void ReadCode(string code)
        {
            codeBytes = CodeToBytes(code);
        }

        public byte[] CodeToBytes(string code)
        {
            var bytes = new List<byte>();
            var codeLength = code.Length;
            var startIndex = -1;
            bool isNote = false;
            //读取代码 多读一位 最后一位用' '代替
            for (int i = 0; i < codeLength + 1; i++)
            {
                //注释
                var c = i < codeLength ? code[i] : ' ';
                if(startIndex == -1)
                {
                    //注释开始
                    if(c == '/')
                    {
                        startIndex = i;
                        isNote = true;
                    }
                    else if(c != ' ' && c != '\n' && c != '\t')
                    {
                        startIndex = i;
                        isNote = false;
                    }
                }
                else
                {
                    if(isNote)
                    {
                        //换行 注释结束
                        if(c == '\n')
                        {
                            startIndex = -1;
                        }
                    }
                    else
                    {
                        if(c == ' ' || c == '\n' || c == '\t')
                        {
                            var fistChar = code[startIndex];
                            var len = i - startIndex;
                            bool isNum = fistChar >= '0' && fistChar <= '9';
                            var strVal = code.Substring(startIndex, len);
                            if(isNum)
                            {
                                if(int.TryParse(strVal, out var num))
                                {
                                    bytes.AddRange(System.BitConverter.GetBytes(num));
                                    UnityEngine.Debug.Log($"读到数字{strVal}");
                                }
                                else
                                {
                                    throw new System.Exception($"{strVal} 不是 数字");
                                }
                            }
                            else
                            {
                                if(symbolDic.TryGetValue(strVal.ToLower(), out var value))
                                {
                                    bytes.Add(value);
                                    UnityEngine.Debug.Log($"读到方法{strVal}");
                                }
                                else
                                {
                                    throw new System.Exception($"{strVal} 不是 方法名");
                                }
                            }
                            startIndex = -1;
                        }
                    }
                }
            }
            
            

            return bytes.ToArray();
        }

        //code
        protected int PopIntFromeCodeBytes()
        {
            var value = System.BitConverter.ToInt32(codeBytes, codePtr);
            codePtr += 4;
            return value;
        }
        
        protected byte PopByteFromeCodeBytes()
        {
            var value = codeBytes[codePtr];
            codePtr ++;
            return value;
        }

        /// <summary>
        /// 用与方法体开始时获取函数指针
        /// </summary>
        /// <returns></returns>
        protected int GetCurrentMethodPtr()
        {
            return codePtr - 1;
        }

        protected int GetPtr()
        {
            return codePtr;
        }

        protected void SetPrt(int _ptr)
        {
            codePtr = _ptr;
        }

        //int
        protected void Push(int value)
        {
            numStack.Push(value);
        }
        //obj
        protected void Push(object value)
        {
            var pos = objArray.Push(value);
            numStack.Push(pos);
        }

        protected int PopInt()
        {
            return numStack.Pop();
        }

        protected T Pop<T>() where T : class
        {
            var pos = numStack.Pop();
            var value = objArray.Get(pos) as T;
            return value;
        } 

        [VMMethod]
        public void Add()
        {
            var right = PopInt();
            var left = PopInt();
            var sum = right + left;
            Push(sum);    
        }
        
        [VMMethod]
        public void Num()
        {
            var num = PopIntFromeCodeBytes();
            Push(num);
        }
        
        //timer time
        
    }
}