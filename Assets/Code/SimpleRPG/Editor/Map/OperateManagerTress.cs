using NullFramework.Runtime;

namespace NullFramework.Editor
{
    //所有操作管理者的枝条
    public class OperateManagerTress : Tress, ILeafDataReciver
    {
        private MapData mapData;
        public MapData CurrentMapData { get => mapData; }
        private OperateManagerTressData leafData;

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
    }

}

