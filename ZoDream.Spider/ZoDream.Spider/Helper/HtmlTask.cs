using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ZoDream.Helper.Http;
using ZoDream.Spider.Model;
using System.IO;
using HtmlAgilityPack;
using ZoDream.Helper.Local;
using Microsoft.International.Converters.TraditionalChineseToSimplifiedConverter;
using ZoDream.Spider.Helper.Http;

namespace ZoDream.Spider.Helper
{
    public class HtmlTask
    {
        public HtmlObject Html { get; set; }

        public IList<RuleItem> Rules { get; set; }

        public IDictionary<string, string> Matches { get; set; } = new Dictionary<string, string>();

        public string Error { get; set; }

        /// <summary>
        /// 默认保存位置
        /// </summary>
        public string FullFile { get; set; }

        public Uri Url { get; set; }

        public SpiderRequest Spider;

        public HtmlTask()
        {

        }

        public HtmlTask(HtmlObject html, IList<RuleItem> rules)
        {
            Html = html;
            Rules = rules;
        }

        public bool Run()
        {
            if (Rules.Count == 0)
            {
                Rules.Add(new RuleItem(RuleKinds.保存, FullFile));
            }
            foreach (var item in Rules)
            {
                switch (item.Kind)
                {
                    case RuleKinds.正则截取:
                        if (string.IsNullOrWhiteSpace(item.Value2))
                        {
                            Html.Narrow(item.Value1);
                        }
                        else if (Regex.IsMatch(item.Value2, "^[0-9]+$"))
                        {
                            Html.NarrowWithTag(item.Value1, int.Parse(item.Value2));
                        }
                        else
                        {
                            Html.NarrowWithTag(item.Value1, item.Value2);
                        }
                        break;
                    case RuleKinds.普通截取:
                        Html.Narrow(item.Value1, item.Value2);
                        break;
                    case RuleKinds.普通替换:
                        Html.Replace(item.Value1, item.Value2);
                        break;
                    case RuleKinds.正则替换:
                        Html.RegexReplace(item.Value1, item.Value2);
                        break;
                    case RuleKinds.正则匹配:
                        Html.Match(item.Value1, item.Value2);
                        break;
                    case RuleKinds.合并网页:
                        MergeHtml(item.Value1);
                        break;
                    case RuleKinds.替换HTML:
                        Html.HtmlToText();
                        break;
                    case RuleKinds.简繁转换:
                        Html.TraditionalToSimplified(string.IsNullOrWhiteSpace(item.Value1) ||
                                                item.Value1.Equals("Y", StringComparison.CurrentCultureIgnoreCase));
                        break;
                    case RuleKinds.XPath选择:
                        Html.XpathChoose(item.Value1, string.IsNullOrWhiteSpace(item.Value2) || item.Value2.Equals("Y", StringComparison.CurrentCultureIgnoreCase));
                        break;
                    case RuleKinds.保存:
                        SaveFile(GetFile(item.Value1), item.Value2);
                        break;
                    case RuleKinds.Csv保存:
                        SaveCsv(item.Value1, GetFile(item.Value2));
                        break;
                    case RuleKinds.Excel保存:
                        SaveExcel(item.Value1, GetFile(item.Value2));
                        break;
                    case RuleKinds.追加:
                        AppendFile(GetFile(item.Value1), item.Value2);
                        break;
                    case RuleKinds.导入:
                        ImportHtml(GetFile(item.Value1), item.Value2);
                        break;
                    default:
                        break;
                }
            }
            try
            {
                var kind = Rules.Last().Kind;
                if (kind != RuleKinds.保存 && kind != RuleKinds.追加 && kind != RuleKinds.导入)
                {
                    SaveFile(GetFile(FullFile));
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                SaveFile(FullFile, "");
            }
            return string.IsNullOrEmpty(Error);
        }


        public string GetFile(string file)
        {
            if (string.IsNullOrWhiteSpace(file))
            {
                file = FullFile;
            }
            file = file.Replace("{path}",
                FullFile)
                .Replace("{host}", Url.Host);
            var matches = Regex.Matches(file, @"\{([^\{\}]+)\}");
            file = matches.Cast<Match>().Where(match => Matches.ContainsKey(match.Groups[1].Value)).Aggregate(file, (current, match) => current.Replace(match.Value, Matches[match.Groups[1].Value]));
            if (file.IndexOf(":\\", StringComparison.Ordinal) < 0)
            {
                file = SpiderHelper.BaseDirectory + "\\" + file.TrimStart('\\');
            }
            FileHelper.CreateDirectory(file);
            return file;
        }

        public void MergeHtml(string tag)
        {
            if (Spider == null)
            {
                return;
            }
            foreach (HtmlValue item in Html)
            {
                if (item.Empty())
                {
                    continue;
                }
                if (!item.Data.ContainsKey(tag))
                {
                    continue;
                }
                var path = item.Data[tag];
                if (string.IsNullOrWhiteSpace(path))
                {
                    continue;
                }
                var uri = new Uri(Url, path).ToString();
                Spider.Results.Add(new UrlTask(uri) { Kind = AssetKind.Html });
            }
        }


        public void AppendFile(string file, string templateFile = "")
        {
            var fs = new FileStream(file, FileMode.Append);
            using (var writer = new StreamWriter(fs, Encoding.UTF8))
            {
                writer.Write(GetContent(GetTemplate(templateFile)));
            }
        }

        public void SaveCsv(string pattern, string file)
        {
            var regex = new Regex(pattern, RegexOptions.IgnoreCase);
            var tags = regex.GetGroupNames();
            var csv = new Csv(file);
            csv.WriteRow(tags);
            foreach (var val in Html)
            {
                var ms = regex.Matches(val.ToString());
                foreach (Match item in ms)
                {
                    foreach (var tag in tags)
                    {
                        csv.AppendColumn(item.Groups[tag].Value);
                    }
                    csv.WriteRow();
                }
            }
            csv.Close();
        }

        public void SaveExcel(string pattern, string file)
        {
            var regex = new Regex(pattern, RegexOptions.IgnoreCase);
            var tags = regex.GetGroupNames();
            var excel = new ExcelHelper();
            excel.Create();
            var sheet = excel.AddSheet("sheet1");
            excel.SetRow(sheet, tags);
            foreach (var val in Html)
            {
                var ms = regex.Matches(val.ToString());
                var i = 0;
                foreach (Match item in ms)
                {
                    var args = tags.Select(tag => item.Groups[tag].Value).ToList();
                    excel.SetRow(sheet, tags, i++);
                }
            }
            excel.SaveAs(file);
            excel.Close();
        }

        public void SaveFile(string file, string templateFile = "")
        {
            var fs = new FileStream(file, FileMode.Create);
            using (var writer = new StreamWriter(fs, Encoding.UTF8))
            {
                writer.BaseStream.Position = writer.BaseStream.Length;
                writer.WriteLine();
                writer.Write(GetContent(GetTemplate(templateFile)));
            }
        }
       

        public void ImportHtml(string url, string param)
        {
            var request = new Request(url);
            if (string.IsNullOrWhiteSpace(param))
            {
                request.Post(Html.ToJson(), "application/json");
                return;
            }
            request.Post(param.Replace("{content}", System.Web.HttpUtility.UrlEncode(Html.ToJson())));
        }

        public string GetContent(string template)
        {
            if (template == "{content}")
            {
                return Html.ToString();
            }
            var matches = Regex.Matches(template, @"\{([^\{\}]+)\}");
            template = matches.Cast<Match>().Where(match => Matches.ContainsKey(match.Groups[1].Value)).Aggregate(template, (current, match) => current.Replace(match.Value, Matches[match.Groups[1].Value]));
            return template.Replace("{content}", Html.ToString());
        }

        public string GetTemplate(string file, string defaultTemplate = "{content}")
        {
            if (!string.IsNullOrWhiteSpace(file) && File.Exists(file))
            {
                return Open.Read(file);
            }
            return defaultTemplate;
        }

    }
   

    
}
