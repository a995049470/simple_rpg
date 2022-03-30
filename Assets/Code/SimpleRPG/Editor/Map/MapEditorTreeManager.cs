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
            Root.Instance.AddMsg(new Msg(MapEditorMsgKind.EditorFinish));
            Root.Instance.Update();
            Root.Dispose();
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
            var worldPosition = start + t * dir;
            var worldIntPosition = new Vector3Int(Mathf.RoundToInt(worldPosition.x), Mathf.RoundToInt(worldPosition.y), Mathf.RoundToInt(worldPosition.z));

            var data = new MsgData_MapEditorEvent();
            data.currentEvent = e;
            data.mouseWorldPosition = worldPosition;
            data.mouseWorldIntPosition = worldIntPosition;
            data.camera = camera;
            var msg = new Msg(MapEditorMsgKind.MapEditorEvent, data);
            Root.Instance.AddMsg(msg);
            Root.Instance.Update();
        }
        
        // private void Update() {
        //     Root.Instance.Update(Time.deltaTime);
        // }
    }

}

