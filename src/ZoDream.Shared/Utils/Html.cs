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
    }
}
