using System;
using System.Text.RegularExpressions;

namespace ZoDream.Helper.Base
{
    public class Text
    {
        public string Content { get; set; }

        public Text()
        {

        }

        public Text(string text)
        {
            Content = text;
        }


        /// <summary>
        /// 缩小范围 使用正则截取
        /// </summary>
        /// <param name="pattern"></param>
        /// <returns></returns>
        public Text Narrow(string pattern)
        {
            Content = Regex.Match(Content, pattern).Value;
            return this;
        }

        public Text NarrowWithTag(string pattern, string tag)
        {
            Content = GetMatch(pattern, tag);
            return this;
        }

        public Text NarrowWithTag(string pattern, int tag)
        {
            Content = GetMatch(pattern, tag);
            return this;
        }

        /// <summary>
        /// 缩小范围 使用index方法截取 不包括
        /// </summary>
        /// <param name="begin"></param>
        /// <param name="end"></param>
        /// <returns></returns>
        public Text Narrow(string begin, string end)
        {
            var index = Content.IndexOf(begin, StringComparison.Ordinal);
            if (index < 0)
            {
                index = 0;
            } else
            {
                index += begin.Length;
            }
            var next = Math.Min(Content.IndexOf(end, index, StringComparison.Ordinal), Content.Length - index);
            Content = Content.Substring(index, next);
            return this;
        }

        public bool IsMatch(string pattern)
        {
            return Regex.IsMatch(Content, pattern);
        }

        public Match Match(string pattern)
        {
            return Regex.Match(Content, pattern, RegexOptions.IgnoreCase);
        }

        public MatchCollection Matches(string pattern)
        {
            return Regex.Matches(Content, pattern, RegexOptions.IgnoreCase);
        }

        public string GetMatch(string pattern, string tag)
        {
            return Match(pattern).Groups[tag].Value;
        }

        public string GetMatch(string pattern, int tag)
        {
            return Match(pattern).Groups[tag].Value;
        }

        public MatchCollection GetMatches(string pattern)
        {
            return Regex.Matches(Content, pattern);
        }

        public Match GetMatch(string pattern)
        {
            return Regex.Match(Content, pattern);
        }

        public Text Replace(string search, string text = "")
        {
            Content = Content.Replace(search, text);
            return this;
        }

        public Text RegexReplace(string search, string text = "")
        {
            Content = Regex.Replace(Content, search, text, RegexOptions.IgnoreCase);
            return this;
        }
    }
}
