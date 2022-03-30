using UnityEngine;
using NullFramework.Runtime;
using SimpleRPG.Runtime;

namespace NullFramework.Editor
{
    public class SingleDeleteLeaf : Leaf
    {
        protected override void InitListeners()
        {
            base.InitListeners();
            AddMsgListener(MapEditorMsgKind.MapEditorEvent, Delete);
        }

        private System.Action Delete(Msg msg)
        {
            var msgData = msg.GetData<MsgData_MapEditorEvent>();
            var e = msgData.currentEvent;
            if(e.isMouse && e.button == 1 && e.type == EventType.MouseDown && !e.alt && !e.control && !e.command)
            {
                msgData.mapData.TrayRmoveBuild(msgData.mouseWorldIntPosition);
            }
            return null;
        }
    }

}

