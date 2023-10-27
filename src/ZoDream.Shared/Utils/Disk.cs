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
            var i = url.IndexOf("//");
            var host = string.Empty;
            if (i >= 0)
            {
                url = url.Substring(i + 2).TrimStart('/');
                i = url.IndexOf('/');
                if (i > 0)
                {
                    host = url.Substring(0, i);
                    url = url.Substring(i + 1);
                } else
                {
                    host = url;
                    url = string.Empty;
                }
            }
            url = url.TrimStart('/');
            i = url.IndexOf('#');
            if (i >= 0)
            {
                url = url.Substring(0, i);
            }
            i = url.IndexOf('?');
            var query = string.Empty;
            if (i >= 0)
            {
                query = url.Substring(i + 1);
                url = url.Substring(0, i);
            }
            var pageExt = string.Empty;
            var m = Regex.Match(url, @"\.[\w]+$");
            var fileName = url;
            if (m.Success)
            {
                pageExt = m.Value;
                fileName = url.Substring(0, url.Length - pageExt.Length);
            }
            fileName = $"{fileName.Trim()}{query}".Trim();
            var ext = pageExt switch
            {
                ".asp" or ".aspx" or ".php" or "" => ".html",
                 _ => pageExt
            };
            if (string.IsNullOrWhiteSpace(fileName) || fileName.EndsWith("/"))
            {
                fileName  += "index";
            }
            if (fileName.Length > 255)
            {
                fileName = fileName.Substring(0, 255 - ext.Length);
            }
            if (!string.IsNullOrWhiteSpace(host))
            {
                fileName = $"{host}\\{fileName}";
            }
            return $"{fileName}{ext}".Replace('/', '\\').ToLower();
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
