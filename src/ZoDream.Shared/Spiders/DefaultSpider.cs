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
using ZoDream.Shared.Interfaces;
using ZoDream.Shared.Models;
using ZoDream.Shared.Providers;

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
        private readonly object _object = new object();


        public SpiderOption Option { get; set; } = new SpiderOption();
        public IUrlProvider UrlProvider { get; set; } = new UrlProvider();
        public IRuleProvider RuleProvider { get; set; } = new RuleProvider();

        public IProxyProvider ProxyProvider { get; set; } = new ProxyProvider();

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
            throw new NotImplementedException();
        }

        public void Resume()
        {
            throw new NotImplementedException();
        }

        public void Start()
        {
            throw new NotImplementedException();
        }

        public void Stop()
        {
            throw new NotImplementedException();
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
                        tasks[i] = new Task(() =>
                        {
                            UrlProvider.UpdateItem(item, UriStatus.DOING);
                            try
                            {
                                // TODO 执行具体规则
                                RunTask(item);
                                UrlProvider.UpdateItem(item, UriStatus.DONE);
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
                    break;
                }
            }, token);
            #endregion
        }

        protected void RunTask(UriItem item)
        {

        }
    }
}
