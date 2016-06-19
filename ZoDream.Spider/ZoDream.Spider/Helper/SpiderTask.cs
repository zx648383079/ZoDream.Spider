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

        private static readonly char[] InvalidFileNameChars = new[] {'"', '<', '>', '|', '\0', '\u0001', '\u0002',
                    '\u0003', '\u0004', '\u0005', '\u0006', '\a', '\b', '\t', '\n', '\v', '\f', '\r', '\u000e',
                    '\u000f', '\u0010', '\u0011', '\u0012', '\u0013', '\u0014', '\u0015', '\u0016', '\u0017',
                    '\u0018', '\u0019',  '\u001a',  '\u001b', '\u001c', '\u001d', '\u001e', '\u001f',  ':', '*',
                    '?', '\\', '/'};

        public List<string> Urls = new List<string>();
        
        public bool Start(string url)
        {
            var html = Download(url);
            if (string.IsNullOrWhiteSpace(html))
            {
                return false;
            }
            var htmler = new Html(html);
            // 提取所有链接 并替换链接为完整链接
            var matches = htmler.GetMatches(@"(src|href)\s?=\s?""?([^""\s<>]*)""?\s?");
            foreach (Match item in matches)
            {
                var arg = GetAbsolute(url, item.Groups[2].Value);
                Urls.Add(arg);
                var args = GetRelative(arg);
                htmler.Replace(item.Groups[0].Value, item.Groups[0].Value.Replace(item.Groups[2].Value, args[0]));
            }
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

        public string GetAbsolute(string url, string relative)
        {
            return new Uri(new Uri(url), relative).ToString();
        }


        public string[] GetRelative(string url)
        {
            var m = Regex.Match(url, @"//[^/]+/(.*?)/?(([^/\?]*?(\.[\w]+)?(\?.+?)?)(#.+)?)$");
            //m.Groups[1].Value 文件路径  
            //m.Groups[2].Value 文件名 
            //m.Groups[3].Value 文件名 不包含 #部分
            //m.Groups[4].Value 拓展名 
            //m.Groups[5].Value 参数 
            //m.Groups[6].Value #id部分
            var fileName = m.Groups[3].Value;
            var ext = string.IsNullOrEmpty(m.Groups[4].Value) ? ".html" : m.Groups[4].Value;
            if (!string.IsNullOrEmpty(m.Groups[5].Value))
            {
                fileName = GetFileName(m.Groups[3].Value) + ext;
            }
            if (string.IsNullOrEmpty(fileName))
            {
                fileName = "index.html";
            }

            if (fileName == ext)
            {
                fileName = "index" + ext;
            }
            if (fileName.Length > 255)
            {
                fileName = fileName.Substring(0, 255 - ext.Length) + ext;
            }
            var path = fileName;
            var relativeUrl = fileName;
            if (!string.IsNullOrEmpty(m.Groups[1].Value))
            {
                relativeUrl = m.Groups[1].Value + '/' + fileName;
                path = m.Groups[1].Value.Replace('/', '\\') + '\\' + fileName;
            }
            return new string[] { relativeUrl, path, fileName };
        }

        public string GetFileName(string fileName)
        {
            fileName = InvalidFileNameChars.Aggregate(fileName, (current, c) => current.Replace(c + "", ""));
            if (fileName.Length > 1)
            {
                if (fileName[0] == '.')
                {
                    fileName = "index" + fileName.TrimStart('.');
                }
            }
            return fileName;    
        }

    }
}
