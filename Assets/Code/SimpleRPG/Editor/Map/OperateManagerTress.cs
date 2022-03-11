using NullFramework.Runtime;

namespace NullFramework.Editor
{
    //所有操作管理者的枝条
    public class OperateManagerTress : Tress<OperateManagerTressData>
    {
        private MapData mapData;
        public MapData CurrentMapData { get => mapData; }

        public override void OnReciveDataFinish()
        {
            mapData = new MapData();
            mapData.SetCurrentPrefab(leafData.DefaultPrefab);
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

