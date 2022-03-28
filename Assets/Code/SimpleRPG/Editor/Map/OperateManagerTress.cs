using NullFramework.Runtime;
using SimpleRPG.Runtime;

namespace NullFramework.Editor
{
    //所有操作管理者的枝条
    public class OperateManagerTress : Tress, ILeafDataReciver, ILeafMemberDicGetter
    {
        private MapData mapData;
        public MapData CurrentMapData { get => mapData; }
        private OperateManagerTressData leafData;
        private LeafMemberDic membderDic;

        public void OnReciveDataFinish()
        {
            mapData = new MapData();
            mapData.SetCurrentPrefab(leafData.DefaultPrefab);
        }

        public void SetLeafData(LeafData data)
        {
            this.leafData = data as OperateManagerTressData;
        }



        protected override void InitListeners()
        {
            base.InitListeners();
            AddMsgListener(MapEditorMsgKind.EditorFinish, OnEditorFinish);
        }

        private void OnEditorFinish(Msg msg)
        {
            mapData.OnEditorFinish();
        }

        public LeafMemberDic GetMemberDic()
        {
            if(membderDic == null)
            {
                membderDic = new LeafMemberDic();
                membderDic[MemberKind.mapData] = mapData;
            }
            return membderDic;
        }
    }

}

