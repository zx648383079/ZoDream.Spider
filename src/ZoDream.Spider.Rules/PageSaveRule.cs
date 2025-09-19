using AngleSharp.Io;
using System;
using System.ComponentModel;
using System.IO;
using System.Net;
using System.Net.Http.Headers;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ZoDream.Shared.Form;
using ZoDream.Shared.Interfaces;
using ZoDream.Shared.Models;
using ZoDream.Shared.Rules.Values;
using ZoDream.Shared.Utils;

namespace ZoDream.Spider.Rules
{
    public class PageSaveRule : IRule, IRuleSaver, IRequestHost
    {

        private ISpiderContainer _container;
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
            _container = container;
            if (UseContentType && !container.IsDebug)
            {
                await SaveWithContentTypeAsync(container);
            } else
            {
                await SaveWithTypeAsync(container);
            }
            _container = null;
        }

        public async Task InvokeAsync(string url, IHttpResponse response)
        {
            if (_container is null)
            {
                return;
            }
            if (url == _container.Url.Source)
            {
                await SaveWithContentTypeAsync(_container, response);
                return;
            }
            var uriType = ParseType(response.ContentTypeMediaType);
            if (!_container.Application.RuleProvider.Cannable(url, uriType))
            {
                return;
            }
            _container.Application.UrlProvider.Add(_container.Url.Level + 1, url, uriType);
            await _container.Application.InvokeAsync(url, response);
        }

        public bool Cannable(string url, UriType type)
        {
            if (_container is null)
            {
                return false;
            }
            if (url == _container.Url.Source)
            {
                return true;
            }
            return _container.Application.RuleProvider.Cannable(url, type);
        }

        public bool TrySave(string url, out string outputPath)
        {
            outputPath = string.Empty;
            if (_container is null)
            {
                return false;
            }
            var storage = _container.Application.Storage;
            if (url == _container.Url.Source)
            {
                outputPath = storage.GetAbsolutePath(GetFileName(url));
                return true;
            }
            var uriType = UriType.File;
            if (!_container.Application.RuleProvider.Cannable(url, uriType))
            {
                return false;
            }
            _container.Application.UrlProvider.Add(_container.Url.Level + 1, url, uriType);
            outputPath = storage.GetAbsolutePath(GetFileName(url));
            return true;
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
            if (container.Data is RuleSource s)
            {
                await SaveWithContentTypeAsync(container, s.Source);
                return;
            }
            var url = container.Url.Source;
            var request = container.Application.GetRequestData(url);
            request.AllowAutoRedirect = false;
            await container.Application.RequestProvider.Getter()
                .SendAsync(request, this);
            //await SaveWithContentTypeAsync(container, response);
        }

        public async Task SaveWithContentTypeAsync(ISpiderContainer container, IHttpResponse response)
        {
            var url = container.Url.Source;
            var storage = container.Application.Storage;
            var fileName = GetFileName(url);
            if (response is null)
            {
                return;
            }
            if (response.StatusCode == HttpStatusCode.Redirect ||
                response.StatusCode == HttpStatusCode.Moved)
            {

                var relativeUri = container.AddUri(response.RedirectLocation, container.Url.Type);
                await storage.CreateAsync(fileName,
                    RenderRedirect(relativeUri)
                    );
                return;
            }
            if (response.StatusCode != HttpStatusCode.OK)
            {
                return;
            }
            var responseFileName = response.ContentDispositionFileName;
            if (!string.IsNullOrWhiteSpace(responseFileName))
            {
                container.Url.Title = responseFileName!;
            }
            var type = ParseType(response.ContentTypeMediaType);
            if (type != UriType.Css && type != UriType.Html)
            {
                var fullPath = storage.GetAbsolutePath(fileName);
                Disk.CreateDirectory(fullPath);
                await response.SaveAsync(fullPath, container.EmitProgress, container.Token);
                return;
            }
            var content = await response.ReadAsync();
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
