namespace ZoDream.Shared.Models
{
    public enum RuleMatchType
    {
        None,
        All,
        Contains,
        Regex,
        Host,
        StartWith,
        Event,
        /// <summary>
        /// 单页包含资源
        /// </summary>
        Page,
    }
}
