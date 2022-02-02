using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNode;

namespace NullFramework.Editor
{   
    /// <summary>
    /// 基础节点 如果Enter 和 Next 节点 相连  说明没有值传递
    /// </summary>
    public abstract class ByteNode : Node
    {
        protected override void Init()
        {
            base.Init();
            //Desc = $"{"ByteNode"}(基节点)";
        }

        public override object GetValue(NodePort port)
        {
            return null;
        }
        

        /// <summary>
        /// 获取代码
        /// </summary>
        /// <param name="isPositve">是否是正面</param>
        /// <returns></returns>
        public virtual List<int> GetCode(bool isPositve)
        {
            return new List<int>();
        }

        public List<int> GetInputCode(string nodeNmae)
        {
            return (GetInputPort(nodeNmae)?.Connection?.node as ByteNode)?.GetCode(false) ?? new List<int>();
        }

        public List<int> GetOutputCode(string nodeNmae)
        {
            return (GetOutputPort(nodeNmae)?.Connection?.node as ByteNode)?.GetCode(true) ?? new List<int>();
        }

    }

}
