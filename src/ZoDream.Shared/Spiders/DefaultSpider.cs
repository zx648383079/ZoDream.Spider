﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Serialization;
using ZoDream.Shared.Interfaces;
using ZoDream.Shared.Models;

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
        public IUrlCollection UrlCollection { get; set; }
        public IRuleProvider RuleProvider { get; set; }

        public IProxyProvider ProxyProvider { get; set; }

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

        public void Pause()
        {
            throw new NotImplementedException();
        }

        public void Resume()
        {
            throw new NotImplementedException();
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
            var tag = "";
            var xml = new StringBuilder();
            string? line;
            var urls = new List<string>();
            string[] args;
            var regex = new Regex(@"^\[(\w+)\]$");
            while (null != (line = reader.ReadLine()))
            {
                if (string.IsNullOrWhiteSpace(line))
                {
                    continue;
                }
                if (regex.IsMatch(line))
                {
                    tag = regex.Match(line).Groups[1].Value.ToUpper();
                    continue;
                }
                switch (tag)
                {
                    case "URL":
                        break;
                    case "REGEX":
                        xml.AppendLine(line);
                        break;
                }
            }
        }

        public void Serializer(StreamWriter writer)
        {
            writer.WriteLine("[URL]");
            UrlCollection.Serializer(writer);
            //foreach (var item in UrlCollection)
            //{
            //    sw.WriteLine($"{item.Status}={item.Type}={item.Source}");
            //}
            writer.WriteLine();
            writer.WriteLine("[OPTION]");
            Option.Serializer(writer);
            writer.WriteLine();
            writer.WriteLine("[REGEX]");
            RuleProvider.Serializer(writer);
        }
    }
}
