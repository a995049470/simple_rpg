namespace NullFramework.Runtime
{
    public class ByteCodeAttribute : System.Attribute
    {
        public int Id;
        public string Desc;
        public bool IsHasEnter;
        public bool IsHasNext;
        public string[] Ports;
        /// <summary>
        /// 是否需要自动生成
        /// </summary>
        public bool IsAuto;
        /// <summary>
        /// 需要自动生成编辑节点的方法
        /// </summary>
        public ByteCodeAttribute(int id, string desc, bool isHasEnter, bool isHasExit, params string[] ports)
        {
            this.Id = id;
            this.Desc = desc;
            this.IsHasEnter = isHasEnter;
            this.IsHasNext = isHasExit;
            this.Ports = ports ?? new string[0];
            this.IsAuto = true;
        }

        /// <summary>
        /// 不需要自动生成编辑节点的方法
        /// </summary>
        public ByteCodeAttribute(int id, string desc)
        {
            this.Id = id;
            this.Desc = desc;
            this.IsAuto = false;
        }
    }
}
