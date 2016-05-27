using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZoDream.Spider.Model
{
    [Serializable]
    public class RuleItem
    {
        public RuleKinds Kind { get; set; }

        public string Value1 { get; set; }

        public string Value2 { get; set; }
        
        public RuleItem()
        {

        }

        public RuleItem(RuleKinds kind, string value)
        {
            Kind = kind;
            Value1 = value;
        }

        public RuleItem(RuleKinds kind, string value1, string value2)
        {
            Kind = kind;
            Value1 = value1;
            Value2 = value2;
        }

    }

    public enum RuleKinds
    {
        正则截取,
        普通截取,
        普通替换,
        正则替换,
        正则匹配,
        保存,
        导入
    }
}
