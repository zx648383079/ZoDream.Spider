using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Serialization;
using ZoDream.Shared.Events;
using ZoDream.Shared.Interfaces;
using ZoDream.Shared.Loggers;
using ZoDream.Shared.Models;
using ZoDream.Shared.Providers;
using ZoDream.Shared.Rules.Values;
using ZoDream.Shared.Spiders.Containers;

namespace ZoDream.Shared.Spiders
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

        public DefaultSpider()
        {
            UrlProvider = new UrlProvider(this);
            RequestProvider = new RequestProvider(this);
        }

        public bool IsDebug { get; set; } = false;

        public bool Paused { get; private set; } = true;
        public SpiderOption Option { get; set; } = new SpiderOption();
        public IUrlProvider UrlProvider { get; set; }
        public IRuleProvider RuleProvider { get; set; } = new RuleProvider();

        public IProxyProvider ProxyProvider { get; set; } = new ProxyProvider();

        public IRequestProvider RequestProvider {  get; set; }

        public ILogger Logger { get; set; } = new FileLogger();

        public event PausedEventHandler PausedChanged;
        public void Load(string file)
        {
            if (string.IsNullOrEmpty(file))
            {
                return;
            }
            using (var sr = new StreamReader(file))
            {
                Deserializer(sr);
            }
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
            } else
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
                        tasks[i] = new Task(async () =>
                        {
                            try
                            {
                                // TODO 执行具体规则
                                await RunTaskAsync(item);
                            }
                            catch (Exception ex)
                            {
                                Debug.WriteLine($"{item.Source}, {ex.Message},{ex.TargetSite}");
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
                    Paused = true;
                    PausedChanged?.Invoke(Paused);
                    break;
                }
            }, token);
            #endregion
        }

        protected async Task RunTaskAsync(UriItem url)
        {
            var items = await GetContainerAsync(url);
            UrlProvider.UpdateItem(url, UriStatus.DOING);
            foreach (var item in items)
            {
                item.Next();
            }
            UrlProvider.UpdateItem(url, UriStatus.DONE);
        }

        public async Task<IList<ISpiderContainer>> GetContainerAsync(UriItem url)
        {
            var items = new List<ISpiderContainer>();
            var rules = RuleProvider.Get(url.Source);
            var shouldPrepare = false;
            foreach (var item in rules)
            {
                var container = new SpiderContainer(this)
                {
                    Url = url,
                };
                foreach (var rule in item.Rules)
                {
                    if (rule == null)
                    {
                        continue;
                    }
                    var r = RuleProvider.Render(rule);
                    if (r == null)
                    {
                        continue;
                    }
                    container.Rules.Add(r);
                    if (r is not IRuleSaver || (r as IRuleSaver).ShouldPrepare)
                    {
                        shouldPrepare = true;
                    }
                }
                items.Add(container);
            }
            if (!shouldPrepare)
            {
                return items;
            }
            var content = await RequestProvider.Getter().GetAsync(url.Source);
            if (content == null)
            {
                items.Clear();
                return items;
            }
            url.Title = MatchTitle(content);
            foreach (var item in items)
            {
                item.Data = new RuleString(content);
            }
            return items;
        }

        private string MatchTitle(string content)
        {
            if (string.IsNullOrEmpty(content))
            {
                return string.Empty;
            }
            var match = Regex.Match(content, @"\<title\>([\s\S]+?)\</title\>");
            if (match == null)
            {
                return string.Empty;
            }
            return match.Groups[1].Value.Trim();
        }
    }
}
