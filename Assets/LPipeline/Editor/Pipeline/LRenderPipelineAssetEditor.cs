
using UnityEditor;
using Sirenix.OdinInspector.Editor;
using LPipeline.Runtime;
using UnityEngine;

namespace LPipeline.Editor
{
    
    [CustomEditor(typeof(LRenderPipelineAsset))]
    public class LRenderPipelineAssetEditor : OdinEditor {
        private static bool isShowPassState = true;
        private static GUIContent passStateTip = new GUIContent("PassState");
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            var asset = (LRenderPipelineAsset)target;
            var passes = asset.Setting.passes;
            EditorGUILayout.Space();
            isShowPassState = EditorGUILayout.Foldout(isShowPassState, passStateTip);
            if(isShowPassState)
            {
                foreach (var pass in passes)
                {
                    if (pass == null) continue;
                    pass.Enable = EditorGUILayout.Toggle(pass.name, pass.Enable);
                }
            }
        }   
    }

}
