using System.Collections.Generic;

namespace ZoDream.Spider.EgretAssetRule.Models
{
    internal class ResMap
    {
        public IList<GroupItem>? Groups { get; set; }
        public IList<ResItem>? Resources { get; set; }
    }
    internal class GroupItem
    {
        public string Name { get; set; } = string.Empty;
        public string Keys { get; set; } = string.Empty;
    }
    internal class ResItem
    {
        public string Name { get; set; } = string.Empty;

        public string Type { get; set; } = string.Empty;

        public string Url { get; set; } = string.Empty;
    }
}
