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
            if(Selection.activeGameObject != this.gameObject) Selection.activeGameObject = null;
            if(e == null) return;
            var camera = UnityEditor.SceneView.lastActiveSceneView.camera
            ;
            if(camera == null) return;
            var mouseScreenPosition = (Vector3)e.mousePosition;
            mouseScreenPosition.y = camera.pixelHeight - mouseScreenPosition.y;
            mouseScreenPosition.z = 1;
            var start = camera.transform.position;
            var end = camera.ScreenToWorldPoint(mouseScreenPosition);
            var dir = end - start;
            if(dir.y == 0) return;
            var t = -start.y / dir.y;
            var positionWS = start + t * dir;

            var data = new MapEditorEventData();
            data.currentEvent = e;
            data.mousePositionWS = positionWS;
            data.camera = camera;
            var msg = new Msg(MapMsgKind.MapEditorEvent, data);
            Root.Instance.AddMsg(msg);
            Root.Instance.Update();
        }
        
        // private void Update() {
        //     Root.Instance.Update(Time.deltaTime);
        // }
    }

}

