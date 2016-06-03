using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ZoDream.Helper.Http;
using ZoDream.Spider.Model;

namespace ZoDream.Spider.Helper
{
    public class HtmlTask
    {
        public Html Html { get; set; }

        public IList<RuleItem> Rules { get; set; }

        public string Error { get; set; }

        public HtmlTask()
        {

        }

        public HtmlTask(Html html, IList<RuleItem> rules)
        {
            Html = html;
            Rules = rules;
        }

        public bool Run()
        {
            foreach (var item in Rules)
            {
                switch (item.Kind)
                {
                    case RuleKinds.正则截取:
                        if (string.IsNullOrWhiteSpace(item.Value2))
                        {
                            Html.Narrow(item.Value1);
                        } else if (Regex.IsMatch(item.Value2, "^[0-9]+$"))
                        {
                            Html.NarrowWithTag(item.Value1, int.Parse(item.Value2));
                        } else
                        {
                            Html.NarrowWithTag(item.Value1, item.Value2);
                        }
                        break;
                    case RuleKinds.普通截取:
                        Html.Narrow(item.Value1, item.Value2);
                        break;
                    case RuleKinds.普通替换:
                        Html.Replace(item.Value1, item.Value2);
                        break;
                    case RuleKinds.正则替换:
                        Html.RegexReplace(item.Value1, item.Value2);
                        break;
                    case RuleKinds.正则匹配:

                        break;
                    case RuleKinds.保存:
                        break;
                    case RuleKinds.导入:
                        break;
                    default:
                        break;
                }
            }
            return String.IsNullOrEmpty(Error);
        }

    }
}
