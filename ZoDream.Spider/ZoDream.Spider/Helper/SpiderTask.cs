using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ZoDream.Helper.Http;

namespace ZoDream.Spider.Helper
{
    public class SpiderTask
    {
        
        public bool Start(string url)
        {
            var html = Download(url);
            if (string.IsNullOrWhiteSpace(html))
            {
                return false;
            }
            var htmler = new Html(html);
            // 提取所有链接 并替换链接为完整链接
            var result = true;
            foreach (var item in SpiderHelper.UrlRegex)
            {
                if (!Regex.IsMatch(url, item.Url))
                {
                    continue;
                }
                var task = new HtmlTask(htmler, item.Rults);
                if (!task.Run())
                {
                    result = false;
                }
            }
            return result;
        }

        public string Download(string url)
        {
            return "";
        }
        

    }
}
