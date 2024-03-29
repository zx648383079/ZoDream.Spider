﻿using AngleSharp.Dom;
using System;
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
    public class LazySpider<T> : ISpider
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

        public Task ExecuteAsync(UriItem url)
        {
            Stop();
            PausedChanged?.Invoke(Paused = false);
            return Task.CompletedTask;
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
            return new SpiderContainer(this, url, rules);
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

        public void InvokeReady(UriItem url, T host)
        {
            InvokeReady(GetContainer(url), host);
        }
        public void InvokeReady(IList<ISpiderContainer> items, T host)
        {
            foreach (var item in items)
            {
                foreach (var it in item.Rules)
                {
                    if (it is IRuleCustomLoader<T> o)
                    {
                        o.Ready(host);
                    }
                }
            }
        }

        public void InvokeDestroy(UriItem url, T host)
        {
            InvokeDestroy(GetContainer(url), host);
        }
        public void InvokeDestroy(IList<ISpiderContainer> items, T host)
        {
            foreach (var item in items)
            {
                foreach (var it in item.Rules)
                {
                    if (it is IRuleCustomLoader<T> o)
                    {
                        o.Destroy(host);
                    }
                }
            }
        }
    }
}
