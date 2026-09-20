using System.Text.Json.Serialization;
using ZoDream.Shared.Converters;

namespace ZoDream.Shared.Models
{
    public class DataItem
    {
        public string Name { get; set; } = string.Empty;
        [JsonConverter(typeof(ObjectToPrimitiveConverter))]
        public object? Value { get; set; }

        public DataItem()
        {
            
        }

        public DataItem(string name, object val)
        {
            Name = name;
            Value = val;
        }
    }
}
