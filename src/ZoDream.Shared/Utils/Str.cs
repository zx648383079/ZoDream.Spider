using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ZoDream.Shared.Utils
{
    public static class Str
    {
        public static string ReplaceCallback(string content, string pattern, Func<Match, string> func)
        {
            return ReplaceCallback(content, new Regex(pattern), func);
        }

        public static string ReplaceCallback(string content, Regex regex, Func<Match, string> func)
        {
            var matches = regex.Matches(content);
            if (matches.Count == 0)
            {
                return content;
            }
            var sb = new StringBuilder();
            var lastIndex = 0;
            foreach (Match item in matches)
            {
                sb.Append(content.Substring(lastIndex, item.Index - lastIndex));
                lastIndex = item.Index + 1;
                sb.Append(func.Invoke(item));
            }
            sb.Append(content.Substring(lastIndex));
            return sb.ToString();
        }

        public static int ToInt(string v)
        {
            if (string.IsNullOrWhiteSpace(v))
            {
                return 0;
            }
            if (int.TryParse(v, out var res))
            {
                return res;
            }
            return 0;
        }
    }
}
