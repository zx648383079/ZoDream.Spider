﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using ZoDream.Shared.Events;
using ZoDream.Shared.Interfaces;
using ZoDream.Shared.Models;
using ZoDream.Shared.Rules.Values;
using ZoDream.Shared.Storage;
using ZoDream.Shared.Utils;
using ZoDream.Spider.Providers;

namespace ZoDream.Spider.Programs
{
    public class DefaultSpider : ISpider
    {
        /// <summary>
        /// 多线程控制
        /// </summary>
        private CancellationTokenSource? _tokenSource;

        /// <summary>
        /// 线程锁
        /// </summary>
        private readonly object _lock = new object();

        public DefaultSpider(IPluginLoader plugin) : this(null, plugin)
        {

        }

        public DefaultSpider(ILogger? logger, IPluginLoader plugin)
        {
            UrlProvider = new UrlProvider(this);
            RequestProvider = new RequestProvider(this);
            RuleProvider = new RuleProvider(this);
            Storage = new StorageProvider(this);
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
        public SpiderOption Option { get; set; } = new SpiderOption();

        public IStorageProvider<string, string, FileStream> Storage { get; set; }

        public IUrlProvider UrlProvider { get; set; }
        public IRuleProvider RuleProvider { get; set; }

        public IProxyProvider ProxyProvider { get; set; } = new ProxyProvider();

        public IRequestProvider RequestProvider { get; set; }

        public IPluginLoader PluginLoader { get; set; }

        public ILogger? Logger { get; private set; }

        public event PausedEventHandler? PausedChanged;
        public void Load(string file)
        {
            if (string.IsNullOrEmpty(file))
            {
                return;
            }
            using var sr = LocationStorage.Reader(file);
            Deserializer(sr);
        }

        public Task LoadAsync(string file)
        {
            return Task.Factory.StartNew(() => {
                Load(file);
            });
        }

        public void Save(string file)
        {
            if (string.IsNullOrEmpty(file))
            {
                return;
            }
            using (var sw = new StreamWriter(file, false, new UTF8Encoding(false)))
            {
                Serializer(sw);
            }
        }

        public Task SaveAsync(string file)
        {
            return Task.Factory.StartNew(() =>
            {
                Save(file);
            });
        }

        public void Pause()
        {
            if (Paused)
            {
                return;
            }
            if (_tokenSource != null)
            {
                _tokenSource.Cancel();
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


        public void Deserializer(StreamReader reader)
        {
            if (reader == null)
            {
                return;
            }
            string? line;
            var regex = new Regex(@"^\[(\w+)\]$");
            while (null != (line = reader.ReadLine()))
            {
                if (string.IsNullOrWhiteSpace(line))
                {
                    continue;
                }
                if (!regex.IsMatch(line))
                {

                    continue;
                }
                var tag = regex.Match(line).Groups[1].Value.ToUpper();
                switch (tag)
                {
                    case "OPTION":
                        Option.Deserializer(reader);
                        break;
                    case "PROXY":
                        ProxyProvider.Deserializer(reader);
                        break;
                    case "RULE":
                        RuleProvider.Deserializer(reader);
                        break;
                    case "URL":
                        UrlProvider.Deserializer(reader);
                        break;
                    default:
                        break;
                }
            }
        }

        public void Serializer(StreamWriter writer)
        {
            writer.WriteLine("[OPTION]");
            Option.Serializer(writer);
            writer.WriteLine();
            writer.WriteLine("[PROXY]");
            ProxyProvider.Serializer(writer);
            writer.WriteLine();
            writer.WriteLine("[RULE]");
            RuleProvider.Serializer(writer);
            writer.WriteLine();
            writer.WriteLine("[URL]");
            UrlProvider.Serializer(writer);
            writer.WriteLine();
        }

        protected void RunTask()
        {
            #region 创造主线程，去分配多个下载线程
            _tokenSource = new CancellationTokenSource();
            var token = _tokenSource.Token;
            Task.Factory.StartNew(() =>
            {

                while (!token.IsCancellationRequested)
                {
                    #region 创建执行下载的线程数组
                    var items = UrlProvider.GetItems(Option.MaxCount);
                    var tasksLength = items.Count;
                    var tasks = new Task[tasksLength];
                    for (var i = 0; i < tasksLength; i++)
                    {
                        var item = items[i];
                        UrlProvider.UpdateItem(item, UriStatus.DOING);
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
                                UrlProvider.UpdateItem(item, UriStatus.ERROR);
                            }
                        });
                    }
                    #endregion

                    #region 监视线程数组完成
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
                UrlProvider.UpdateItem(url, UriStatus.ERROR);
                return;
            }
            Logger?.Info($"{url.Source} has {items.Count} rule groups");
            UrlProvider.UpdateItem(url, UriStatus.DOING);
            foreach (var item in items)
            {
                await item.NextAsync();
                if (Paused)
                {
                    UrlProvider.UpdateItem(url, UriStatus.NONE);
                    return;
                }
            }
            UrlProvider.UpdateItem(url, UriStatus.DONE);
        }

        public ISpiderContainer GetContainer(UriItem url, IList<IRule> rules)
        {
            return new SpiderContainer(this, url, rules);
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
            if (!shouldPrepare)
            {
                return items;
            }
            var content = await RequestProvider.Getter().GetAsync(url.Source, Option.HeaderItems, ProxyProvider.Get());
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
            UrlProvider.UpdateItem(uri, UriStatus.DOING);
            var success = true;
            Paused = false;
            PausedChanged?.Invoke(Paused);
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
                    Logger?.Error(ex.Message);
                }
            }
            UrlProvider.UpdateItem(uri, success ? UriStatus.DONE : UriStatus.ERROR);
            Paused = true;
            PausedChanged?.Invoke(Paused);
        }
    }
}
