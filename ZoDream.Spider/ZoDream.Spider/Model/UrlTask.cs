using System;
using System.Linq;
using GalaSoft.MvvmLight;
using System.Text.RegularExpressions;
using ZoDream.Spider.Helper;

namespace ZoDream.Spider.Model
{
    public class UrlTask: ObservableObject
    {
        public string Url { get; set; }

        public string FileName { get; set; }

        public string FullName { get; set; }

        public AssetKind Kind { get; set; }


        private UrlStatus _status;

        /// <summary>
        /// Sets and gets the Status property.
        /// Changes to that property's value raise the PropertyChanged event. 
        /// </summary>
        public UrlStatus Status
        {
            get
            {
                return _status;
            }

            set
            {
                if (_status == value)
                {
                    return;
                }

                _status = value;
                RaisePropertyChanged();
            }
        }

        public UrlTask()
        {

        }

        public UrlTask(string url)
        {
            Url = url;
            GetRelative();
        }

        public UrlTask(string url, string file)
        {
            Url = url;
            FullName = file;
        }

        public UrlTask(string url, string file, string fileName)
        {
            Url = url;
            FullName = file;
            FileName = fileName;
        }

        public void GetRelative()
        {
            var m = Regex.Match(Url, @"//(.*?)/?(([^/\?]*?(\.[\w]+)?(\?.+?)?)(#.+)?)$");
            //m.Groups[1].Value 文件路径  
            //m.Groups[2].Value 文件名 
            //m.Groups[3].Value 文件名 不包含 #部分
            //m.Groups[4].Value 拓展名 
            //m.Groups[5].Value 参数 
            //m.Groups[6].Value #id部分
            FileName = m.Groups[3].Value;
            var ext = string.IsNullOrEmpty(m.Groups[4].Value) ? ".html" : m.Groups[4].Value;
            if (!string.IsNullOrEmpty(m.Groups[5].Value))
            {
                FileName = GetFileName(m.Groups[3].Value) + ext;
            }
            if (string.IsNullOrEmpty(FileName))
            {
                FileName = "index.html";
            }

            if (FileName == ext)
            {
                FileName = "index" + ext;
            }
            if (FileName.Length > 255)
            {
                FileName = FileName.Substring(0, 255 - ext.Length) + ext;
            }
            FullName = SpiderHelper.BaseDirectory + "\\" + (string.IsNullOrEmpty(m.Groups[1].Value) ? "" : m.Groups[1].Value.Replace('/', '\\') + '\\') + FileName;
        }

        public string GetFileName(string fileName)
        {
            fileName = InvalidFileNameChars.Aggregate(fileName, (current, c) => current.Replace(c + "", ""));
            if (fileName.Length <= 1) return fileName;
            if (fileName[0] == '.')
            {
                fileName = "index" + fileName.TrimStart('.');
            }
            return fileName;
        }

        private static readonly char[] InvalidFileNameChars = {'"', '<', '>', '|', '\0', '\u0001', '\u0002',
            '\u0003', '\u0004', '\u0005', '\u0006', '\a', '\b', '\t', '\n', '\v', '\f', '\r', '\u000e',
            '\u000f', '\u0010', '\u0011', '\u0012', '\u0013', '\u0014', '\u0015', '\u0016', '\u0017',
            '\u0018', '\u0019',  '\u001a',  '\u001b', '\u001c', '\u001d', '\u001e', '\u001f',  ':', '*',
            '?', '\\', '/'};
    }

    public enum AssetKind
    {
        Html,
        Js,
        Css,
        Image,
        File
    }
}
