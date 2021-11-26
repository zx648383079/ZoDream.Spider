using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ZoDream.Shared.Utils
{
    public static class Html
    {
        public static string MatchTitle(string content)
        {
            if (string.IsNullOrEmpty(content))
            {
                return string.Empty;
            }
            var match = Regex.Match(content, @"\<title\>([\s\S]+?)\</title\>");
            if (match == null)
            {
                return string.Empty;
            }
            return match.Groups[1].Value.Trim();
        }

        public static IList<string> GenerateUrl(string template)
        {
            var sourceItems = new List<string> { template };
            var matches = Regex.Matches(template, @"\$\{(.+?)\}");
            if (matches == null)
            {
                return sourceItems;
            }
            for (int i = matches.Count - 1; i >= 0; i--)
            {
                var match = matches[i];
                var con = match.Groups[1].Value;
                if (con.IndexOf("...") > 0)
                {
                    sourceItems = GenerateIntUrl(sourceItems, con, match.Index, match.Index + match.Value.Length);
                    continue;
                }
                if (con.IndexOf(',') > 0)
                {
                    sourceItems = GenerateMapUrl(sourceItems, con, match.Index, match.Index + match.Value.Length);
                    continue;
                }
            }
            return sourceItems;
        }

        private static List<string> GenerateMapUrl(List<string> items, string con, int startPosition, int endPosition)
        {
            var data = new List<string>();
            foreach (var val in con.Split(','))
            {
                foreach (var item in items)
                {
                    data.Add(item.Substring(0, startPosition) + val + item.Substring(endPosition));
                }
            }
            return data;
        }

        private static List<string> GenerateIntUrl(List<string> items, string con, int startPosition, int endPosition)
        {
            var args = con.Split(new string[] { "..." }, StringSplitOptions.None);
            var start = Str.ToInt(args[0]);
            var step = 1;
            var end = Str.ToInt(args[1]);
            if (args.Length >= 3)
            {
                step = end - start;
                end = Str.ToInt(args[2]);
            }
            if (step == 0 || (step > 0 && end < start) || (step < 0 && end > start))
            {
                return items;
            }
            var data = new List<string>();
            while (true)
            {
                foreach (var item in items)
                {
                    data.Add(item.Substring(0, startPosition) + start + item.Substring(endPosition));
                }
                start += step;
                if ((step > 0 && end < start) || (step < 0 && end > start))
                {
                    break;
                }
            }
            return data;
        }

        public static string ToText(string content)
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

        public static string ReplaceHtml(string html)
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
