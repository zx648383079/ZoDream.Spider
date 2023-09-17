using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ZoDream.Shared.Interfaces;
using ZoDream.Shared.Models;
using ZoDream.Shared.Utils;

namespace ZoDream.Spider.Rules
{
    public class PageSaveRule : IRule, IRuleSaver
    {
        private string FileName = string.Empty;
        private string Template = string.Empty;
        public PluginInfo Info()
        {
            return new PluginInfo("网页保存");
        }

        public bool ShouldPrepare { get; } = false;
        public bool CanNext { get; } = false;
        public void Ready(RuleItem option)
        {
            FileName = option.Param1.Trim();
            Template = option.Param2.Trim();
        }
        public string GetFileName(string url)
        {
            var path = Disk.RenderFile(url);
            if (string.IsNullOrEmpty(FileName))
            {
                return path;
            }
            var uri = new Uri(url);
            return Regex.Replace(FileName, @"\$\{([a-zA-Z0-9_]+)\}", match => {
                return match.Groups[1].Value switch
                {
                    "host" => uri.Host,
                    "path" => path,
                    "md5" => Md5.Encode(url),
                    _ => match.Value,
                };
            });
        }


        public async Task RenderAsync(ISpiderContainer container)
        {
            var url = container.Url.Source;
            var storage = container.Application.Storage;
            var fileName = GetFileName(url);
            var type = container.Url.Type;
            if (type != UriType.Css && type != UriType.Html)
            {
                await container.GetAsync(
                        storage.GetAbsolutePath(fileName),
                        url);
                return;
            }
            var content = await container.GetAsync(url);
            if (content is null)
            {
                return;
            }
            using var fs = await storage.CreateStreamAsync(fileName);
            using var writer = new StreamWriter(fs, new UTF8Encoding(false));
            var finder = new HtmlUrlRule();
            if (container.Url.Type == UriType.Css)
            {
                finder.GetUrlFromCss(container, ref content);
            }
            else
            {
                container.Url.Title = Html.MatchTitle(content);
                finder.GetUrlFromHtml(container, ref content);
            }
            writer.Write(content);
            writer.Flush();
            fs.SetLength(fs.Position);
        }


    }
}
