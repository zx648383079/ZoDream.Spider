using HtmlAgilityPack;
using Microsoft.International.Converters.TraditionalChineseToSimplifiedConverter;
using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using ZoDream.Helper.Http;

namespace ZoDream.Spider.Helper
{
    public class HtmlObject : IEnumerator, IEnumerable
    {
        protected HtmlValue content;

        protected List<HtmlValue> data;

        protected bool isArray = false;

        private int position = -1;

        public HtmlObject(string content)
        {
            this.content = new HtmlValue(content);
        }

        public HtmlObject(IEnumerable<string> args)
        {
            data = args.Select(item => new HtmlValue(item)).ToList();
            isArray = true;
        }

        public int Count()
        {
            return isArray ? data.Count() : 1;
        }

        public object Current => isArray ? data[position] : content;

        public IEnumerator GetEnumerator()
        {
            return this;
        }


        public bool MoveNext()
        {
            position++;
            return position < Count();
        }

        public void Reset()
        {
            position = -1;
        }

        public void Dispose()
        {
            content = null;
            if (data != null)
            {
                data.Clear();
            }
        }

        public void SetItem(string val) 
        {
            if (isArray)
            {
                data[position].Content = val;
                return;
            }
            if (content == null)
            {
                content = new HtmlValue(val);
                return;
            }
            content.Content = val;
        }

        public void SetItem(HtmlValue val)
        {
            if (!isArray)
            {
                content = val;
                return;
            }
            data[position] = val;
        }

        public void Narrow(string pattern)
        {
            var regex = new Regex(pattern, RegexOptions.IgnoreCase);
            foreach (var val in this)
            {
                SetItem(regex.Match(val.ToString()).Value);
            }
        }

        public void NarrowWithTag(string pattern, int tag)
        {
            var regex = new Regex(pattern, RegexOptions.IgnoreCase);
            foreach (var val in this)
            {
                SetItem(regex.Match(val.ToString()).Groups[tag].Value);
            }
        }

        public void Narrow(string begin, string end)
        {
            foreach (var item in this)
            {
                var val = item.ToString();
                var index = val.IndexOf(begin, StringComparison.Ordinal);
                if (index < 0)
                {
                    index = 0;
                }
                else
                {
                    index += begin.Length;
                }
                var next = Math.Min(val.IndexOf(end, index, StringComparison.Ordinal), val.Length - index);
                SetItem(val.Substring(index, next));
            }
        }

        public void Replace(string search, string text = "")
        {
            foreach (var val in this)
            {
                SetItem(val.ToString().Replace(search, text));
            }
        }

        public void RegexReplace(string search, string text = "")
        {
            var regex = new Regex(search, RegexOptions.IgnoreCase);
            foreach (var val in this)
            {
                SetItem(regex.Replace(val.ToString(), text));
            }
        }

        public void NarrowWithTag(string pattern, string tag)
        {
            var regex = new Regex(pattern, RegexOptions.IgnoreCase);
            foreach (var val in this)
            {
                SetItem(regex.Match(val.ToString()).Groups[tag].Value);
            }
        }

        public void HtmlToText()
        {
            var helper = new Html();
            foreach (var val in this)
            {
                helper.Content = val.ToString();
                helper.GetText();
                SetItem(helper.Content);
            }
        }

        public void TraditionalToSimplified(bool isTc)
        {
            foreach (var val in this)
            {
                SetItem(ChineseConverter.Convert(val.ToString(), isTc ? ChineseConversionDirection.TraditionalToSimplified : ChineseConversionDirection.SimplifiedToTraditional));
            }
        }

        public void Match(string pattern, string tag)
        {
            var regex = new Regex(pattern, RegexOptions.IgnoreCase);
            var items = new List<HtmlValue>();
            var isEmpty = string.IsNullOrWhiteSpace(tag);
            var tagNum = !isEmpty && Regex.IsMatch(tag, "^[0-9]+$") ? int.Parse(tag) : -1;
            var tags = regex.GetGroupNames();
            foreach (var val in this)
            {
                var match = regex.Match(val.ToString());
                if (isEmpty)
                {
                    items.Add(new HtmlValue(tags, match));
                } else if (tagNum >= 0)
                {
                    items.Add(new HtmlValue(match.Groups[tagNum].Value));
                } else
                {
                    items.Add(new HtmlValue(match.Groups[tag].Value));
                }
            }
            isArray = true;
            data = items;
        }

        public void XpathChoose(string tag, bool isHtml)
        {
            var items = new List<HtmlValue>();
            var doc = new HtmlDocument();
            foreach (var val in this)
            {
                doc.LoadHtml(val.ToString());
                var nodes = doc.DocumentNode.SelectNodes(tag);
                if (nodes == null || nodes.Count == 0)
                {
                    return;
                }
                foreach (var node in nodes)
                {
                    items.Add(new HtmlValue(isHtml ? node.InnerHtml : node.InnerText));
                }
            }
            isArray = true;
            data = items;
        }

        public override string ToString()
        {
            var sb = new StringBuilder();
            foreach(var val in this)
            {
                sb.AppendLine(val.ToString());
            }
            return sb.ToString();
        }

        public string ToJson()
        {
            var json = new JArray();
            foreach (HtmlValue item in this)
            {
                if (item.Empty())
                {
                    json.Add(item.Content);
                } else
                {
                    var obj = new JObject();
                    foreach (var val in item.Data)
                    {
                        obj.Add(val.Key, val.Value);
                    }
                    json.Add(item);
                }
            }
            return json.ToString();
        }

        public static explicit operator string(HtmlObject v)
        {
            return v.ToString();
        }
    }
}
