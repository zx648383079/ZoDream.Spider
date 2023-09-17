using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ZoDream.Shared.Utils
{
    public static class Disk
    {
        private static readonly char[] InvalidFileNameChars = {'"', '<', '>', '|', '\0', '\u0001', '\u0002',
            '\u0003', '\u0004', '\u0005', '\u0006', '\a', '\b', '\t', '\n', '\v', '\f', '\r', '\u000e',
            '\u000f', '\u0010', '\u0011', '\u0012', '\u0013', '\u0014', '\u0015', '\u0016', '\u0017',
            '\u0018', '\u0019',  '\u001a',  '\u001b', '\u001c', '\u001d', '\u001e', '\u001f',  ':', '*',
            '?', '\\', '/'};

        public static string RenderFile(string url)
        {
            var m = Regex.Match(url, @"//(.*?)/?(([^/\?]*?(\.[\w]+)?(\?.+?)?)(#.+)?)$");
            //m.Groups[1].Value 文件路径  
            //m.Groups[2].Value 文件名 
            //m.Groups[3].Value 文件名 不包含 #部分
            //m.Groups[4].Value 拓展名 
            //m.Groups[5].Value 参数 
            //m.Groups[6].Value #id部分
            var fileName = m.Groups[3].Value;
            var pageExt = m.Groups[4].Value.ToLower();
            var ext = pageExt switch
            {
                ".asp" or ".aspx" or ".php" => ".html",
                 _ => pageExt
            };
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
            return (string.IsNullOrEmpty(m.Groups[1].Value) ? "" : m.Groups[1].Value.Replace('/', '\\') + '\\') + fileName;
        }
        public static string GetFileName(string fileName)
        {
            fileName = InvalidFileNameChars.Aggregate(fileName, (current, c) => current.Replace(c + "", ""));
            if (fileName.Length <= 1) return fileName;
            if (fileName[0] == '.')
            {
                fileName = "index" + fileName.TrimStart('.');
            }
            return fileName;
        }

        public static void CreateDirectory(string fileName)
        {
            if (File.Exists(fileName))
            {
                return;
            }
            //判断路径中的文件夹是否存在
            var fileFolder = fileName.Substring(0, fileName.LastIndexOf('\\'));
            var pathItems = fileFolder.Split('\\');
            if (pathItems.Length <= 1) return;
            var path = pathItems[0];
            for (var i = 1; i < pathItems.Length; i++)
            {
                path += "\\" + pathItems[i];
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
            }
        }
    }
}
