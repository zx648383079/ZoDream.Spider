using AngleSharp.Io;
using System;
using System.IO;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ZoDream.Shared.Form;
using ZoDream.Shared.Interfaces;
using ZoDream.Shared.Models;
using ZoDream.Shared.Utils;

namespace ZoDream.Spider.Rules
{
    public class PageSaveRule : IRule, IRuleSaver
    {
        private string FileName = string.Empty;

        private bool UseContentType = false;
        public PluginInfo Info()
        {
            return new PluginInfo("网页保存");
        }

        public bool ShouldPrepare { get; } = false;
        public bool CanNext { get; } = false;

        public IFormInput[]? Form()
        {
            return new IFormInput[] {
                Input.Text(nameof(FileName), "保存地址"),
                Input.Switch(nameof(UseContentType), "开启内容判断(不支持浏览器)"),
            };
        }
        public void Ready(RuleItem option)
        {
            FileName = option.Get<string>(nameof(FileName)) ?? string.Empty;
            UseContentType = option.Get<bool>(nameof(UseContentType));
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
            if (UseContentType && !container.IsDebug)
            {
                await SaveWithContentTypeAsync(container);
                return;
            }
            await SaveWithTypeAsync(container);
        }
        /// <summary>
        /// 根据推测的类型判断
        /// </summary>
        /// <param name="container"></param>
        /// <returns></returns>
        public async Task SaveWithTypeAsync(ISpiderContainer container)
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
        /// <summary>
        /// 直接根据响应头判断
        /// </summary>
        /// <param name="container"></param>
        /// <returns></returns>
        public async Task SaveWithContentTypeAsync(ISpiderContainer container)
        {
            var url = container.Url.Source;
            var storage = container.Application.Storage;
            var fileName = GetFileName(url);
            var request = container.Application.GetRequestData(url);
            request.AllowAutoRedirect = false;
            using var client = container.Application.RequestProvider.Getter()
                .Create(request);
            using var response = await client.ReadResponseAsync();
            if (response is null) {
                return;
            }
            if (response.StatusCode == HttpStatusCode.Redirect || 
                response.StatusCode == HttpStatusCode.Moved)
            {
                
                var relativeUri = container.AddUri(response.Headers.Location.ToString(), container.Url.Type);
                await storage.CreateAsync(fileName, 
                    RenderRedirect(relativeUri)
                    );
                return;
            }
            if (response.StatusCode != HttpStatusCode.OK)
            {
                return;
            }
            var responseFileName = response.Content.Headers.ContentDisposition?.FileName;
            if (!string.IsNullOrWhiteSpace(responseFileName))
            {
                container.Url.Title = responseFileName!;
            }
            var type = ParseType(response.Content.Headers.ContentType?.MediaType);
            if (type != UriType.Css && type != UriType.Html)
            {
                var fullPath = storage.GetAbsolutePath(fileName);
                Disk.CreateDirectory(fullPath);
                await client.SaveAsync(response,
                    fullPath, container.EmitProgress, container.Token);
                return;
            }
            var content = await client.ReadAsync(response);
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

        private UriType ParseType(string? contentType)
        {
            if (contentType == "text/css")
            {
                return UriType.Css;
            }
            if (contentType == "text/html")
            {
                return UriType.Html;
            }
            return UriType.File;
        }

        private string RenderRedirect(string url)
        {
            return $"<!DOCTYPE html><html><head><meta http-equiv=\"refresh\" content=\"0.1;url={url}\"></head></html>";
        }

        private UriType ParseType(HttpResponseHeaders headers)
        {
            if (!headers.TryGetValues("Content-Type", out var header))
            {
                return UriType.File;
            }
            foreach (var item in header)
            {
                if (item == "text/css")
                {
                    return UriType.Css;
                }
                if (item == "text/html")
                {
                    return UriType.Html;
                }
            }
            return UriType.File;
        }
    }
}
