using System;
using System.Text;
using System.Text.RegularExpressions;

namespace ZoDream.Helper.Http
{
    public class Url
    {
        public static string GetUrl(string url)
        {
            return GetUrl(url, SearchKind.Baidu);
        }

        public static string GetUrl(string url, SearchKind kind)
        {
            return IsUrl(url) ? AddHead(url) : GetSearchUrl(url, kind);
        }

        public static string GetSearchUrl(string word, SearchKind kind)
        {
            word = UrlEncode(word);
            switch (kind)
            {
                case SearchKind.Baidu:
                    return "https://www.baidu.com/baidu?tn=SE_zzsearchcode_shhzc78w&word=" + word;
                case SearchKind.Google:
                    return "https://www.google.com.hk/?gws_rd=ssl#newwindow=1&safe=strict&q=" + word;
                case SearchKind.Bing:
                    return "http://cn.bing.com/search?q=" + word;
                case SearchKind.So:
                    return "https://www.so.com/s?q=" + word;
                case SearchKind.Sogou:
                    return "http://www.sogou.com/web?query=" + word;
            }
            return word;
        }

        public static bool IsUrl(string url)
        {
            var regex = new Regex(@"(//)?[^/\.]+\.[^/\.]+");
            //给网址去所有空格
            var m = regex.Match(url);
            return m.Success;
        }

        public static string AddHead(string url)
        {
            var index = url.IndexOf("//", StringComparison.Ordinal);
            if (index < 0 || index > 6)
            {
                return "http://" + url;
            }
            switch (index)
            {
                case 0:
                    return "http:" + url;
                case 1:
                    return "http" + url;
            }
            return url;
        }

        public static string UrlEncode(string str)
        {
            var sb = new StringBuilder();
            var byStr = Encoding.UTF8.GetBytes(str); //默认是System.Text.Encoding.Default.GetBytes(str)
            foreach (var t in byStr)
            {
                sb.Append(@"%" + Convert.ToString(t, 16));
            }
            return sb.ToString();
        }
    }

    public enum SearchKind
    {
        Baidu,
        Google,
        Bing,
        So,
        Sogou
    }
}
