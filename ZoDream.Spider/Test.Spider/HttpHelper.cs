using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ZoDream.Helper.Http;
using ZoDream.Helper.Local;

namespace Test.Spider
{
    public class HttpHelper
    {
        public HttpHelper(string url)
        {
            AddFile(url);
            _saveDirectory = GetDirectory();
        }

        public HttpHelper(string url, string file)
        {
            AddFile(url);
            SaveDirectory = file;
        }

        public List<string> Urls { get; set; }

        private string _saveDirectory;

        public string SaveDirectory
        {
            get {
                return _saveDirectory;
            }
            set {
                _saveDirectory = GetDirectory(value);
            }
        }

        public string GetDirectory()
        {
            return GetDirectory(DateTime.Now.ToShortDateString());
        }

        public string GetDirectory(string file)
        {
            if (Directory.Exists(file))
            {
                return file;
            }
            return string.Format("{0}\\{1}", Directory.GetCurrentDirectory(), file);
        }

        public void AddFile(string file)
        {
            if (File.Exists(file))
            {
                AddUrlByFile(file);
                return;
            }
            var tempFile = Directory.GetCurrentDirectory() + '\\' + file;
            if (File.Exists(tempFile))
            {
                AddUrlByFile(tempFile);
                return;
            }
            var index = file.IndexOf("//");
            if (index < 0)
            {
                AddUrl("http://" + file);
                return;
            }
            if (index == 1)
            {
                AddUrl("http" + file);
                return;
            }
            if (index == 0)
            {
                AddUrl("http:" + file);
                return;
            }
            AddUrl(file);
            return;
        }

        public void AddUrlByFile(string file)
        {
            if (!File.Exists(file))
            {
                return;
            }
            using (var sr = new StreamReader(file, UTF8Encoding.UTF8))
            {
                string line;
                while (null != (line = sr.ReadLine()))
                {
                    if (string.IsNullOrWhiteSpace(line))
                    {
                        continue;
                    }
                    AddUrl(line);
                }
            }
        }

        public Task Start()
        {
            return Task.Factory.StartNew(() => {
                foreach (var item in Urls)
                {
                    Download(item);
                }
            });
        }

        public void AddUrl(string url)
        {
            Urls.Add(url);
        }

        public void Download(string url)
        {
            var http = new Request();
            var html = http.Get(url);
            var title = Regex.Match(html, @"<title>([^\<\>]+)</title>").Groups[1].Value;
            if (string.IsNullOrWhiteSpace(title))
            {
                title = Regex.Match(url, @"([^/]+)", RegexOptions.RightToLeft).Value;
            }
            Open.Writer(string.Format("{0}\\{1}.html", SaveDirectory, title), html);
        }
    }
}
