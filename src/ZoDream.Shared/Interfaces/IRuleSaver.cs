namespace ZoDream.Shared.Interfaces
{
    /// <summary>
    /// 保存类规则
    /// </summary>
    public interface IRuleSaver
    {
        /// <summary>
        /// 是否需要预处理
        /// </summary>
        public bool ShouldPrepare{ get;}
        /// <summary>
        /// 判断是否会执行下一个规则
        /// </summary>
        public bool CanNext { get; }
        public string GetFileName(string url);
    }
}
