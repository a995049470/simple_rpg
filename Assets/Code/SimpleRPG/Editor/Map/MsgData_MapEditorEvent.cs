using NullFramework.Runtime;
using UnityEngine;

namespace NullFramework.Editor
{
    public class MsgData_MapEditorEvent : MsgData
    {
        public Event currentEvent;
        public Camera camera;
        public Vector3 mouseWorldPosition;
        public Vector3Int mouseWorldIntPosition;
        public MapData mapData;
    }
}

