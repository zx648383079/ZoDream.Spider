using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using ZoDream.Helper.Base;

namespace ZoDream.Helper.Http
{
    public class Html : Text
    {
        public Html()
        {

        }

        public Html(string html):base(html)
        {

        }

        public Html SetUrl(string url)
        {
            var request = new Request();
            Content = request.Get(url);
            return this;
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

        /// <summary>
        /// 自定义替换规则 = ||
        /// </summary>
        /// <param name="replace"></param>
        /// <returns></returns>
        public void Replace(string replace)
        {
            var ms = Regex.Matches(replace, @"([^(=|\|\|)]+)(=([^(=|\|\|)]*))?");
            foreach (Match match in ms)
            {
                Content = Regex.Replace(Content, match.Groups[1].Value, match.Groups[3].Value, RegexOptions.IgnoreCase);
            }
        }

        public List<string> GetLinks()
        {
            return GetLinks("");
        }

        public List<string> GetLinks(string rootUrl)
        {
            var ms = GetMatches(@"\<a[^\<\>]+?[hH][Rr][Ee][fF][\s]?=[\s""]?(?<href>[^""\<\>\s#]+)[^\<\>]+?\>");
            return (from Match item in ms select item.Groups["href"].Value into url where url.IndexOf("javascript:", StringComparison.Ordinal) < 0 select new Uri(new Uri(rootUrl), url) into uri select uri.ToString()).ToList();
        }

        public List<string> GetHref()
        {
            var ms = GetMatches(@"\<[^\<\>]+?[hH][Rr][Ee][fF][\s]?=[\s""]?(?<href>[^""\<\>\s]+)[^\<\>]+?\>");
            return (from Match item in ms select item.Groups["href"].Value).ToList();
        }

        public List<string> GetSrc()
        {
            var ms = GetMatches(@"\<[^\<\>]+?[sS][Rr][Cc][\s]?=[\s""]?(?<src>[^""\<\>\s]+)[^\<\>]+?\>");
            return (from Match item in ms select item.Groups["src"].Value).ToList();
        }
        
        

        /// <summary>
        /// 获取文本
        /// </summary>
        /// <param name="replace">自定义的替换</param>
        /// <returns></returns>
        public string GetText(string replace = null)
        {  
            Content = ReplaceHtml(Content);
            //替换掉 < 和 > 标记
            //_html = _html.Replace("<", "");
            //_html = _html.Replace(">", "");
            if (!string.IsNullOrWhiteSpace(replace))
            {
                Replace(replace);
            }
            // 替换被转义的
            Content = Content.Replace("\\n", "\n")
                .Replace("\\xa1", "\xa1")
                .Replace("\\xa2", "\xa2")
                .Replace("\\xa3", "\xa3")
                .Replace("\\xa9", "\xa9");
            //返回去掉_html标记的字符串
            return Content;
        }

        public bool Comparer(string url, string url2)
        {
            if (url.IndexOf("//", StringComparison.Ordinal) < 0 || url2.IndexOf("//", StringComparison.Ordinal) < 0)
            {
                return true;
            }
            var regex = new Regex(@"//[^/]+");
            return regex.Match(url).Value.Equals(regex.Match(url2).Value, StringComparison.CurrentCultureIgnoreCase);
        }
    }
}
