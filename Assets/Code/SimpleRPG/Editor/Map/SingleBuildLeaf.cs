using UnityEngine;
using NullFramework.Runtime;
using SimpleRPG.Runtime;

namespace NullFramework.Editor
{

    //只负责简单的实例化
    public class SingleBuildLeaf : Leaf
    {

        protected override void InitListeners()
        {
            base.InitListeners();
            AddMsgListener(MapEditorMsgKind.MapEditorEvent, Build);
        }

        private System.Action Build(Msg msg)
        {
            var msgData = msg.GetData<MsgData_MapEditorEvent>();
            var e = msgData.currentEvent;
            if(e.isMouse && e.button == 0 && e.type == EventType.MouseDown && !e.alt && !e.control && !e.command)
            {
                msgData.mapData.TrayAddBuild(msgData.mouseWorldIntPosition);
            }
            return null;
        }

    }

}

