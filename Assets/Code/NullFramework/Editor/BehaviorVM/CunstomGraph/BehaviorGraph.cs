using System.Collections.Generic;
using UnityEngine;
using XNode;

namespace NullFramework.Editor
{
    [CreateAssetMenu]
    public class BehaviorGraph : NodeGraph
    {
        public List<int> GetCode()
        {
            List<int> code = null;
            BehaviorEndNode node = null;
            for (int i = 0; i < nodes.Count; i++)
            {
                node = nodes[i] as BehaviorEndNode;
                if(node != null)
                {
                    break;
                }
            }
            if(node != null)
            {
                code = node.GetCode(false);
            }
            else
            {
                code = new List<int>();
            }
            return code;
        }
    }
    
}
