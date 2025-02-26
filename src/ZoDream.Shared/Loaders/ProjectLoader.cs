﻿using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using ZoDream.Shared.Interfaces;
using ZoDream.Shared.Models;
using ZoDream.Shared.Storage;
using ZoDream.Shared.Utils;

namespace ZoDream.Shared.Loaders
{
    public partial class ProjectLoader : ILoader
    {

        public string FileName { get; set; } = string.Empty;
        /// <summary>
        /// 并发数
        /// </summary>
        public int ParallelCount { get; set; } = 1;
        /// <summary>
        /// 重试次数
        /// </summary>
        public int RetryCount { get; set; } = 1;
        /// <summary>
        /// 重试间隔时间(/s)
        /// </summary>
        public int RetryTime { get; set; } = 0;
        public int TimeOut { get; set; } = 60;
        public bool UseBrowser { get; set; } = false;
        /// <summary>
        /// 工作目录
        /// </summary>
        public string Workspace { get; set; } = string.Empty;

        public List<HeaderItem> HeaderItems { get; set; } = [];

        public List<string> ProxyItems { get; set; } = [];
        public List<string> EntryItems { get; set; } = [];

        public List<HostItem> HostItems { get; set; } = [];

        public List<RuleGroupItem> RuleItems { get; set; } = [];

        public void Deserializer(StreamReader reader)
        {
            Read(reader);
        }

        public void Serializer(StreamWriter writer)
        {
            Write(writer);
        }

        public Task<bool> LoadAsync()
        {
            return Task.Factory.StartNew(() => {
                if (string.IsNullOrEmpty(FileName))
                {
                    return false;
                }
                using var sr = LocationStorage.Reader(FileName);
                Read(sr);
                return true;
            });
        }

        public Task<bool> SaveAsync()
        {
            return Task.Factory.StartNew(() => {
                if (string.IsNullOrEmpty(FileName))
                {
                    return false;
                }
                using var sw = new StreamWriter(FileName, false, new UTF8Encoding(false));
                Write(sw);
                return true;
            });
        }

        public HostItem? GetHostMap(string url)
        {
            if (HostItems.Count == 0)
            {
                return null;
            }
            var host = Html.MatchHost(url);
            if (string.IsNullOrWhiteSpace(host))
            {
                return null;
            }
            foreach (var item in HostItems)
            {
                if (item.Host == host)
                {
                    return item;
                }
            }
            return null;
        }

        public ProjectLoader()
        {
            
        }

        public ProjectLoader(string fileName)
        {
            FileName = fileName;
        }
    }
}
