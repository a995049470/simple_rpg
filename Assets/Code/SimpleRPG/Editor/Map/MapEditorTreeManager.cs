using UnityEngine;
using SimpleRPG.Runtime;
using NullFramework.Runtime;
using UnityEditor;

namespace NullFramework.Editor
{
    [ExecuteAlways]
    public class MapEditorTreeManager : BaseTreeManager
    {   
        
        private void OnEnable() {
            this.gameObject.name = nameof(MapEditorTreeManager);
            InitTree();
            SceneView.duringSceneGui += DuringSceneGUI;
        }

        private void OnDisable() {
            SceneView.duringSceneGui -= DuringSceneGUI;
        }

        private void DuringSceneGUI(SceneView view) {
            var e = Event.current;
            if(e == null) return;
            if(e.isMouse && e.button == 0 && e.type == EventType.MouseDown)
            {
                var camera = SceneView.lastActiveSceneView.camera;
                if(camera == null) return;
                var screenPos = new Vector3(e.mousePosition.x, camera.pixelHeight - e.mousePosition.y, 1);
                var start = camera.transform.position;
                var end = camera.ScreenToWorldPoint(screenPos);
                var dir = end - start;
                if(dir.y == 0) 
                {
                    Debug.LogError("与地面无交点...");
                    return;
                }
                var t = -start.y / dir.y;
                var hitPosition = start + t * dir;
                var data = new MouseClickData();
                data.mousePositionWS = hitPosition;
                var msg = new Msg(MapMsgKind.MouseClick, data);
                Root.Instance.AddMsg(msg);
                e.Use();
            }
            if(Selection.activeGameObject != this.gameObject) Selection.activeGameObject = null;
             
            Root.Instance.Update();
        }
        
        // private void Update() {
        //     Root.Instance.Update(Time.deltaTime);
        // }
    }

}

