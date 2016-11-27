using System.Collections.Generic;
using System.Linq;
using ZoDream.Spider.Helper.Http;
using ZoDream.Spider.Model;
using ZoDream.Spider.View;

namespace ZoDream.Spider.Helper
{
    public class SpiderHelper
    {
        public static List<HeaderItem> Headers = new List<HeaderItem>();

        public static List<UrlItem> UrlRegex = new List<UrlItem>();

        public static int Count = 100;

        public static int TimeOut = 10000;

        public static string BaseDirectory;

        public static bool UseBrowser = false;

        /// <summary>
        /// 所有已存在的网址集合
        /// </summary>
        public static List<string> UrlList = new List<string>();

        public static bool CanAdd(string url)
        {
            return !UrlList.Contains(url) && UrlRegex.Any(item => item.IsMatch(url));
        }

        /// <summary>
        /// 获取所有的规则
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static List<UrlItem> GetRules(string url)
        {
            return UrlRegex.Where(item => item.IsMatch(url)).ToList();
        }

        public static WebView Browser;

        public static WebView GetBrowser()
        {
            if (Browser != null) return Browser;
            Browser = new WebView();
            Browser.Show();
            return Browser;
        }
    }
}
