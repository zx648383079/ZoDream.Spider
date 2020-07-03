using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ZoDream.Spider.Helper
{
    public class HtmlValue
    {
        public string Content { get; set; } = string.Empty;

        public Dictionary<string, string> Data { get; set; } = new Dictionary<string, string>();


        public bool Empty()
        {
            return Data == null || Data.Count() == 0;
        }

        public void Append(HtmlValue html)
        {
            if (html.Empty())
            {
                return;
            }
            Append(html.Data);
        }

        public void Append(Dictionary<string, string> data)
        {
            if (Data == null)
            {
                Data = new Dictionary<string, string>();
            }
            foreach (var item in data)
            {
                Data.Add(item.Key, item.Value);
            }
        }

        public void Append(string key, string val)
        {
            if (Data == null)
            {
                Data = new Dictionary<string, string>();
            }
            Data.Add(key, val);
        }

        public HtmlValue()
        {

        }

        public HtmlValue(string val)
        {
            Content = val;
        }

        public HtmlValue(string[] tags, Match match)
        {
            Content = match.Value;
            foreach (var item in tags)
            {
                Data.Add(item, match.Groups[item].Value);
            }
        }

        public override string ToString()
        {
            return Content;
        }

        public void Dispose()
        {
            Content = string.Empty;
            if (Data != null)
            {
                Data.Clear();
            }
        }
    }
}
