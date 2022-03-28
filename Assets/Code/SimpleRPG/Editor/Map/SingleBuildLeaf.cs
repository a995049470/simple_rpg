using UnityEngine;
using NullFramework.Runtime;
using SimpleRPG.Runtime;

namespace NullFramework.Editor
{

    //只负责简单的实例化
    public class SingleBuildLeaf : Leaf, ILeafMemberDicSetter
    {
        private MapData mapData;

        
        public void ReciveLeaf(Leaf leaf)
        {
            if(leaf is OperateManagerTress tress)
            {
                this.mapData = tress.CurrentMapData;
            }
        }

        public void SetMemberDic(LeafMemberDic dic)
        {
            mapData = dic[MemberKind.mapData] as MapData;
        }

        protected override void InitListeners()
        {
            base.InitListeners();
            AddMsgListener(MapEditorMsgKind.MapEditorEvent, Build);
        }

        private void Build(Msg msg)
        {
            var msgData = msg.GetData<MapEditorEventData>();
            var e = msgData.currentEvent;
            if(e.isMouse && e.button == 0 && e.type == EventType.MouseDown && !e.alt && !e.control && !e.command)
            {
                mapData.TrayAddBuild(msgData.mouseWorldIntPosition);
            }
        }

    }

}

