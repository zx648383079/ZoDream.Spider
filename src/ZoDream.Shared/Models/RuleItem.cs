using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Threading.Tasks;

namespace ZoDream.Shared.Models
{
    public class RuleItem
    {
        public string Name { get; set; } = string.Empty;

        public IList<DataItem> Values { get; set; } = new List<DataItem>();

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
