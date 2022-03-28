namespace NullFramework.Editor
{
    public static class MapEditorMsgKind
    {
        private const int prefix = 1 << 16;
        public static int MapEditorEvent = prefix | 0;
        public static int EditorFinish = prefix | 1;
    }

}

