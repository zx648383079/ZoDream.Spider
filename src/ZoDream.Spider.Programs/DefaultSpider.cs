using AngleSharp.Dom;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ZoDream.Shared.Events;
using ZoDream.Shared.Interfaces;
using ZoDream.Shared.Loaders;
using ZoDream.Shared.Models;
using ZoDream.Shared.Rules.Values;
using ZoDream.Shared.Utils;
using ZoDream.Spider.Providers;

namespace ZoDream.Spider.Programs
{
    public class DefaultSpider : ISpider
    {
        /// <summary>
        /// 多线程控制
        /// </summary>
        private CancellationTokenSource _tokenSource = new();

        /// <summary>
        /// 线程锁
        /// </summary>
        private readonly object _lock = new();

        public DefaultSpider(ProjectLoader loader, IPluginLoader plugin) : this(loader, null, plugin)
        {

        }

        public DefaultSpider(ProjectLoader loader, ILogger? logger, IPluginLoader plugin)
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
            if (Paused)
            {
                return;
            }
            _tokenSource?.Cancel();
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
            _tokenSource = new();
            if (RequestProvider.SupportTask)
            {
                RunTask();
            }
            else
            {
                RunSingleTask();
            }
        }

        private async void RunSingleTask()
        {
            while (UrlProvider.HasMore)
            {
                var items = UrlProvider.GetItems(1);
                if (items == null)
                {
                    break;
                }
                if (Paused)
                {
                    break;
                }
                foreach (var item in items)
                {
                    await RunTaskAsync(item);
                }
            }
            if (!Paused)
            {
                InvokeEvent("done");
            }
            Paused = true;
            PausedChanged?.Invoke(Paused);
        }

        public void Stop()
        {
            Pause();
            UrlProvider.Reset();
        }
        protected void RunTask()
        {
            #region 创造主线程，去分配多个下载线程
            var token = _tokenSource.Token;
            Task.Factory.StartNew(() =>
            {

                while (!token.IsCancellationRequested)
                {
                    #region 创建执行下载的线程数组
                    var items = UrlProvider.GetItems(Project.ParallelCount);
                    var tasksLength = items.Count;
                    var tasks = new Task[tasksLength];
                    for (var i = 0; i < tasksLength; i++)
                    {
                        var item = items[i];
                        UrlProvider.EmitUpdate(item, UriCheckStatus.Doing);
                        tasks[i] = new Task(() =>
                        {
                            try
                            {
                                // TODO 执行具体规则
                                RunTaskAsync(item).GetAwaiter().GetResult();
                            }
                            catch (Exception ex)
                            {
                                var error = $"{item.Source}, {ex.Message},{ex.TargetSite}";
                                Debug.WriteLine(error);
                                Logger?.Error(error);
                                UrlProvider.EmitUpdate(item, UriCheckStatus.Error);
                            }
                        });
                    }
                    #endregion

                    #region 监视线程数组完成
                    if (tasks.Length > 0)
                    {
                        var continuation = Task.Factory.ContinueWhenAll(tasks, (task) =>
                        { }, token);
                        foreach (var task in tasks)
                        {
                            task.Start();
                        }
                        while (!continuation.IsCompleted)
                        {
                            Thread.Sleep(1000);
                        }
                    }
                    #endregion
                    if (UrlProvider.HasMore) continue;
                    _tokenSource.Cancel();
                    InvokeEvent("done");
                    Paused = true;
                    PausedChanged?.Invoke(Paused);
                    break;
                }
            }, token);
            #endregion
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

        protected async Task RunTaskAsync(UriItem url)
        {
            var items = await GetContainerAsync(url);
            if (items.Count < 1)
            {
                Logger?.Info($"{url.Source} has 0 rule groups, jump");
                UrlProvider.EmitUpdate(url, UriCheckStatus.Error);
                return;
            }
            Logger?.Info($"{url.Source} has {items.Count} rule groups");
            UrlProvider.EmitUpdate(url, UriCheckStatus.Doing);
            var i = 0;
            var success = true;
            UrlProvider.EmitProgress(url, i, items.Count);
            var sb = new StringBuilder();
            foreach (var item in items)
            {
                try
                {
                    await item.NextAsync();
                }
                catch (Exception ex)
                {
                    success = false;
                    Logger?.Error($"{url.Source}: {ex.Message}");
                    sb.AppendLine(ex.Message);
                }
                UrlProvider.EmitProgress(url, ++i, items.Count);
                if (Paused)
                {
                    UrlProvider.EmitUpdate(url, UriCheckStatus.None);
                    return;
                }
            }
            UrlProvider.EmitUpdate(url, success ? UriCheckStatus.Done : UriCheckStatus.Error, sb.ToString());
        }

        public async Task ExecuteAsync(UriItem url)
        {
            Stop();
            _tokenSource = new();
            PausedChanged?.Invoke(Paused = false);
            try
            {
                await RunTaskAsync(url);
            }
            catch (Exception ex)
            {
                Logger?.Error(ex.Message);
            }
            PausedChanged?.Invoke(Paused = true);
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
            return new SpiderContainer(this, url, rules, _tokenSource.Token);
        }

        public async Task<IList<ISpiderContainer>> GetContainerAsync(UriItem url)
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
            //if (shouldPrepare && url.Type != UriType.Html && url.Type != UriType.Css && url.Type != UriType.Js)
            //{
            //    shouldPrepare = false;
            //}
            if (!shouldPrepare)
            {
                return items;
            }
            /**
             * TODO 使用 host 的方法：把 网址中的 host 替换为 ip 在请求头中设置 Host 为被替换的域名 
             */
            var content = await RequestProvider.Getter().GetAsync(
                    GetRequestData(url.Source)
                );
            if (content == null)
            {
                Logger?.Waining($"{url.Source} HTML EMPTY");
                items.Clear();
                return items;
            }
            url.Title = Html.MatchTitle(content);
            foreach (var item in items)
            {
                item.Data = new RuleString(content);
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
            var success = true;
            Paused = false;
            PausedChanged?.Invoke(Paused);
            var i = 0;
            UrlProvider.EmitProgress(uri, i, rules.Count);
            foreach (var item in rules)
            {
                var con = GetContainer(uri, PluginLoader.Render(item.Rules));
                con.Data = new RuleString(html);
                try
                {
                    await con.NextAsync();
                }
                catch (Exception ex)
                {
                    success = false;
                    Logger?.Error($"{url}: {ex.Message}");
                }
                UrlProvider.EmitProgress(uri, ++i, rules.Count);
            }
            UrlProvider.EmitUpdate(uri, success ? UriCheckStatus.Done : UriCheckStatus.Error);
            Paused = true;
            PausedChanged?.Invoke(Paused);
        }
    }
}
