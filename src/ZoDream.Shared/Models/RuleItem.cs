using System.Collections.Generic;

namespace ZoDream.Shared.Models
{
    public class RuleItem
    {
        public string Name { get; set; } = string.Empty;

        public IList<DataItem> Values { get; set; } = [];

        public T? Get<T>(string key)
        {
            foreach (var item in Values)
            {
                if (item.Name == key)
                {
                    return (T)item.Value;
                }
            }
            return default;
        }
    }
}
