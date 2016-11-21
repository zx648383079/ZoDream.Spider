using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ZoDream.Helper.Http;
using ZoDream.Spider.Model;
using System.IO;
using ZoDream.Helper.Local;

namespace ZoDream.Spider.Helper
{
    public class HtmlTask
    {
        public Html Html { get; set; }

        public IList<RuleItem> Rules { get; set; }

        public IDictionary<string, string> Matches { get; set; } = new Dictionary<string, string>();

        public string Error { get; set; }

        /// <summary>
        /// 默认保存位置
        /// </summary>
        public string FullFile { get; set; }

        public HtmlTask()
        {

        }

        public HtmlTask(Html html, IList<RuleItem> rules)
        {
            Html = html;
            Rules = rules;
        }

        public bool Run()
        {
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
                        Match(item.Value1);
                        break;
                    case RuleKinds.保存:
                        SaveFile(GetFile(item.Value1), item.Value2);
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
                    SaveFile(FullFile, "");
                }
            }
            catch (Exception)
            {
                SaveFile(FullFile, "");
            }
            return string.IsNullOrEmpty(Error);
        }

        public void Match(string pattern)
        {
            var regex = new Regex(pattern, RegexOptions.IgnoreCase);
            var match = regex.Match(Html.Content);
            foreach (var item in regex.GetGroupNames())
            {
                Matches.Add(item, match.Groups[item].Value);
            }
        }

        public string GetFile(string file)
        {
            var matches = Regex.Matches(file, @"\{([^\{\}]+)\}");
            return matches.Cast<Match>().Where(match => Matches.ContainsKey(match.Groups[1].Value)).Aggregate(file, (current, match) => current.Replace(match.Value, Matches[match.Groups[1].Value]));
        }
             
        public void SaveExcel(string file, MatchCollection matches)
        {

        }

        public void AppendFile(string file, string templateFile)
        {
            var template = "{content}";
            if (File.Exists(templateFile))
            {
                template = Open.Read(templateFile);
            }
            var fs = new FileStream(file, FileMode.Append);
            using (var writer = new StreamWriter(fs, new UTF8Encoding(false)))
            {
                writer.Write(template.Replace("{content}", Html.Content));
            }
        }

        public void SaveFile(string file, string templateFile)
        {
            var template = "{content}";
            if (File.Exists(templateFile))
            {
                template = Open.Read(templateFile);
            }
            if (string.IsNullOrWhiteSpace(file))
            {
                file = FullFile;
            }
            var fs = new FileStream(file, FileMode.Create);
            using (var writer = new StreamWriter(fs, new UTF8Encoding(false)))
            {
                writer.Write(template.Replace("{content}", Html.Content));
            }
        }
       

        public void ImportHtml(string url, string param)
        {
            if (string.IsNullOrWhiteSpace(param))
            {
                param = "content={content}";
            }
            var request = new Request(url);
            request.Post(param.Replace("{content}", Html.Content));
        }

    }
   

    
}
