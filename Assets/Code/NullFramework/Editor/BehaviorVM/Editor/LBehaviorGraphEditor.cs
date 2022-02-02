using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using XNodeEditor;
using UnityEditor;

namespace NullFramework.Editor
{
    [CustomEditor(typeof(BehaviorGraph), true)]
    public class LBehaviorGraphEditor : GlobalGraphEditor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            if (GUILayout.Button("GetCode", GUILayout.Height(40)))
            {
                var graph = this.target as BehaviorGraph;
                var code = graph.GetCode();
                string str = "";
                for (int i = 0; i < code.Count; i++)
                {
                    if(i == code.Count - 1)
                    {
                        str += $"{code[i]} ";
                    }
                    else
                    {
                        str += $"{code[i]}, ";
                    }
                }
                str = $"new int[]{{ {str}}}";
                GUIUtility.systemCopyBuffer = str;
                Debug.Log($"已复制 <color=#ffff00>{str}</color>");
            }

        }
    }
    
}

