using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ZoDream.Shared.Models
{
    public class RuleGroupItem
    {
        public string Name { get; set; } = string.Empty;

        public IList<RuleItem> Rules { get; set; } = new List<RuleItem>();

        [JsonIgnore]
        public string EventName
        {
            get {
                if (!IsEvent)
                {
                    return string.Empty;
                }
                return Name.Substring(6);
            }
        }

        [JsonIgnore]
        public bool IsEvent
        {
            get
            {
                return Name.StartsWith("event:");
            }
        }

        public bool IsMatch(string uri)
        {
            if (IsEvent)
            {
                return false;
            }
            if (Name == "*")
            {
                return true;
            }
            if (Regex.IsMatch(Name, @"^\w+\.\w+(\.\w+)?$"))
            {
                return new Uri(uri).Host == Name;
            }
            try
            {
                return Regex.IsMatch(uri, Name);
            }
            catch (Exception)
            {
                return false;
            }
        }
    }
}
