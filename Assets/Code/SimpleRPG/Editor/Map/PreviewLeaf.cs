using NullFramework.Runtime;
using SimpleRPG.Runtime;

namespace NullFramework.Editor
{
    public class PreviewLeaf : Leaf
    {
        protected override void InitListeners()
        {
            base.InitListeners();
            AddMsgListener(MapEditorMsgKind.MapEditorEvent, Preview);
        }
        private System.Action Preview(Msg msg)
        {
            var msgData = msg.GetData<MsgData_MapEditorEvent>();
            var e = msgData.currentEvent;
            if(e.type == UnityEngine.EventType.MouseMove)
            {
                msgData.mapData.ShowPreview(msgData.mouseWorldIntPosition);
            }
            return null;
        }
    }

}

