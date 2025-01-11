using System;
using System.Text;
using System.Text.RegularExpressions;

namespace ZoDream.Shared.Http
{
    public class UriRender
    {
        public static string Render(string url)
        {
            return Render(url, 0);
        }

        public static string Render(string url, int kind)
        {
            return IsUrl(url) ? AddHead(url) : RenderSearch(url, kind);
        }

        public static string RenderSearch(string word, int kind)
        {
            word = UrlEncode(word);
            switch (kind)
            {
                case 0:
                    return "https://www.baidu.com/s?wd=" + word;
                case 1:
                    return "https://www.google.com/search?q=" + word;
                case 2:
                    return "https://cn.bing.com/search?q=" + word;
                case 3:
                    return "https://www.so.com/s?q=" + word;
                case 4:
                    return "https://www.sogou.com/web?query=" + word;
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
                return "https://" + url;
            }
            switch (index)
            {
                case 0:
                    return "https:" + url;
                case 1:
                    return "https" + url;
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
}
