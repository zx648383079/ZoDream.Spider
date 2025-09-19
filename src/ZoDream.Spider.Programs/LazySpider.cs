using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using ZoDream.Shared.Events;
using ZoDream.Shared.Interfaces;
using ZoDream.Shared.Loaders;
using ZoDream.Shared.Models;
using ZoDream.Spider.Providers;

namespace ZoDream.Spider.Programs
{
    /// <summary>
    /// 需要用户主动调用
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class LazySpider : ISpider
    {
        public LazySpider(ProjectLoader loader, IPluginLoader plugin) : this(loader, null, plugin)
        {

        }

        public LazySpider(ProjectLoader loader, ILogger? logger, IPluginLoader plugin)
        {
            Project = loader;
            UrlProvider = new UrlProvider(this);
            RequestProvider = new RequestProvider(this);
            RuleProvider = new RuleProvider(this);
            ProxyProvider = new ProxyProvider(this);
            Storage = new StorageProvider(this)
            {
                EntranceFile = loader.FileName
            };
            Logger = logger;
            PluginLoader = plugin;
        }


        private IWebView? _browser;
        private UriItem? _lastUri;
        private readonly Dictionary<UriItem, ISpiderContainer> _containerItems = [];
        public bool IsDebug { get; set; } = false;

        private volatile bool _paused = true;
        public bool Paused
        {
            get { return _paused; }
            set { _paused = value; }
        }
        public ProjectLoader Project { get; private set; }

        public IStorageProvider<string, string, FileStream> Storage { get; set; }

        public IUrlProvider UrlProvider { get; set; }
        public IRuleProvider RuleProvider { get; set; }

        public IProxyProvider ProxyProvider { get; set; }

        public IRequestProvider RequestProvider { get; set; }

        public IPluginLoader PluginLoader { get; set; }

        public ILogger? Logger { get; private set; }

        public event PausedEventHandler? PausedChanged;

        public void Pause()
        {
            if (_browser is not null)
            {
                if (_lastUri is not null)
                {
                    InvokeDestroy(_lastUri, _browser);
                }
                _browser.DocumentLoaded -= WebView_DocumentLoaded;
                _browser.DocumentUnLoaded -= WebView_DocumentUnLoaded;
                _browser.DocumentReady -= WebView_DocumentReady;
                _browser.Destroy();
                _browser = null;
            }
            Paused = true;
            PausedChanged?.Invoke(Paused);
        }

        public void Resume()
        {
            Stop();
            Start();
        }

        public void Start()
        {
            if (!Paused)
            {
                return;
            }
            Paused = false;
            PausedChanged?.Invoke(Paused);
        }

        public UriItem? Next()
        {
            var items = UrlProvider.HasMore ? UrlProvider.GetItems(1) : null;
            if (items is null || items.Count == 0)
            {
                if (!Paused)
                {
                    InvokeEvent("done");
                }
                Paused = true;
                PausedChanged?.Invoke(Paused);
                return null;
            }
            return items[0];
        }

        public void Stop()
        {
            Pause();
            _containerItems.Clear();
            UrlProvider.Reset();
        }

        public async void InvokeEvent(string name)
        {
            var items = RuleProvider.GetEvent(name);
            foreach (var item in items)
            {
                var con = GetContainer(new UriItem(), PluginLoader.Render(item.Rules));
                await con.NextAsync();
            }
        }

        public async Task ExecuteAsync(UriItem url)
        {
            Stop();
            if (RequestProvider is not IWebViewProvider p)
            {
                return;
            }
            _browser = p.AsWebView();
            _browser.DocumentLoaded += WebView_DocumentLoaded;
            _browser.DocumentUnLoaded += WebView_DocumentUnLoaded;
            _browser.DocumentReady += WebView_DocumentReady;
            Logger?.Info($"Ready: {url.Source}");
            await _browser.ReadyAsync();
            PausedChanged?.Invoke(Paused = false);
            await _browser.OpenAsync(GetRequestData(url.Source));
        }

        private void WebView_DocumentReady(IWebView sender, string uri)
        {
            var url = UrlProvider.Get(uri);
            if (url is null)
            {
                return;
            }
            url.Title = sender.DocumentTitle;
            UrlProvider.EmitUpdate(url);
        }

        private void WebView_DocumentUnLoaded(IWebView sender, string uri)
        {
            var url = UrlProvider.Get(uri);
            if (url is null)
            {
                return;
            }
            InvokeDestroy(url, sender);
        }

        private void WebView_DocumentLoaded(IWebView sender, string uri)
        {
            
            if (!RuleProvider.Cannable(uri, UriType.Html))
            {
                return;
            }
            var url = UrlProvider.TryAdd(uri, UriType.Html);
            if (url is null)
            {
                return;
            }
            Logger?.Info($"Listening: {uri}");
            InvokeReady(url, sender);
        }

        public RequestData GetRequestData(string url)
        {
            return new RequestData(url,
                    Project.HeaderItems,
                    ProxyProvider.Get(),
                    Project.GetHostMap(url))
            {
                Timeout = Project.TimeOut,
                RetryTime = Project.RetryTime,
                RetryCount = Project.RetryCount,
            };
        }

        public ISpiderContainer GetContainer(UriItem url, IList<IRule> rules)
        {
            if (_containerItems.TryGetValue(url, out var container))
            {
                return container;
            }
            container = new SpiderContainer(this, url, rules);
            _containerItems.Add(url, container);
            return container;
        }

        public IList<ISpiderContainer> GetContainer(UriItem url)
        {
            var items = new List<ISpiderContainer>();
            var rules = RuleProvider.Get(url.Source);
            if (rules == null || rules.Count < 1)
            {
                return items;
            }
            var shouldPrepare = false;
            foreach (var item in rules)
            {
                items.Add(GetContainer(url, PluginLoader.Render(item.Rules, ref shouldPrepare)));
            }
            return items;
        }

        public async Task InvokeAsync(string url, string html)
        {
            var uri = UrlProvider.Get(url);
            if (uri == null)
            {
                Logger?.Error("没有获取到网址");
                return;
            }
            var rules = RuleProvider.Get(url);
            if (rules == null || rules.Count < 1)
            {
                Logger?.Error("没有获取到规则");
                return;
            }
            UrlProvider.EmitUpdate(uri, UriCheckStatus.Doing);
            Paused = false;
            PausedChanged?.Invoke(Paused);
            await Task.CompletedTask;
        }

        public async Task InvokeAsync(string url, IHttpResponse response)
        {
            await InvokeAsync(url, await response.ReadAsync());
        }

        public void InvokeReady(UriItem url, IWebView host)
        {
            InvokeReady(GetContainer(url), host);
        }

        public void InvokeReady(IList<ISpiderContainer> items, IWebView host)
        {
            foreach (var item in items)
            {
                foreach (var it in item.Rules)
                {
                    if (it is IWebViewRule o)
                    {
                        Logger?.Info($"Rule Ready: {it.Info().Name}");
                        o.Ready(host, item);
                    }
                }
            }
        }

        public void InvokeDestroy(UriItem url, IWebView host)
        {
            InvokeDestroy(GetContainer(url), host);
        }
        public void InvokeDestroy(IList<ISpiderContainer> items, IWebView host)
        {
            foreach (var item in items)
            {
                foreach (var it in item.Rules)
                {
                    if (it is IWebViewRule o)
                    {
                        o.Destroy(host);
                    }
                }
            }
        }
    }
}
