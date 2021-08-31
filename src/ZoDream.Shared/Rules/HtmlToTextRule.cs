using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ZoDream.Shared.Interfaces;
using ZoDream.Shared.Rules.Values;

namespace ZoDream.Shared.Rules
{
    public class HtmlToTextRule : IRule
    {
        public void Render(ISpiderContainer container)
        {
            container.Data = container.Data.Select(i => new RuleString(ToText(i.ToString())));
            container.Next();
        }

        public string ToText(string content)
        {
            content = ReplaceHtml(content);
            //替换掉 < 和 > 标记
            //_html = _html.Replace("<", "");
            //_html = _html.Replace(">", "");
            // 替换被转义的
            content = content.Replace("\\n", "\n")
                .Replace("\\xa1", "\xa1")
                .Replace("\\xa2", "\xa2")
                .Replace("\\xa3", "\xa3")
                .Replace("\\xa9", "\xa9");
            //返回去掉_html标记的字符串
            return content;
        }

        public string ReplaceHtml(string html)
        {
            html = Regex.Replace(html, @"\s+", "", RegexOptions.IgnoreCase);
            html = Regex.Replace(html, @"<!--[\s\S]*-->", "", RegexOptions.IgnoreCase);
            html = Regex.Replace(html, @"<(script|style)[^>]*?>.*?</\1>", "", RegexOptions.IgnoreCase);
            html = Regex.Replace(html, @"<(br|p)[^>]*>", "\n", RegexOptions.IgnoreCase);
            html = Regex.Replace(html, @"<[^>]*>", "", RegexOptions.IgnoreCase);
            html = Regex.Replace(html, @"&(quot|#34);", "/", RegexOptions.IgnoreCase);
            html = Regex.Replace(html, @"&(amp|#38);", "&", RegexOptions.IgnoreCase);
            html = Regex.Replace(html, @"&(lt|#60);", "<", RegexOptions.IgnoreCase);
            html = Regex.Replace(html, @"&(gt|#62);", ">", RegexOptions.IgnoreCase);
            html = Regex.Replace(html, @"&(nbsp|#160);", " ", RegexOptions.IgnoreCase);
            html = Regex.Replace(html, @"&(iexcl|#161);", "\xa1", RegexOptions.IgnoreCase);
            html = Regex.Replace(html, @"&(cent|#162);", "\xa2", RegexOptions.IgnoreCase);
            html = Regex.Replace(html, @"&(pound|#163);", "\xa3", RegexOptions.IgnoreCase);
            html = Regex.Replace(html, @"&(copy|#169);", "\xa9", RegexOptions.IgnoreCase);
            html = Regex.Replace(html, @"&#\d+;", "", RegexOptions.IgnoreCase);
            return html;
        }
    }
}
