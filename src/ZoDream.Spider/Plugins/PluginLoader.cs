using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using ZoDream.Shared.Interfaces;
using ZoDream.Shared.Models;

namespace ZoDream.Spider.Plugins
{
    public class PluginLoader: IPluginLoader, IServiceCollection
    {
        internal IDictionary<string, PluginItem> PluginItems = new Dictionary<string, PluginItem>();

        public bool Can(string name)
        {
            if (!PluginItems.TryGetValue(name, out PluginItem? item))
            {
                return false;
            }
            if (item == null || item.Callback == null)
            {
                return false;
            }
            return true;
        }
        public bool Can(RuleItem rule)
        {
            return Can(rule.Name);
        }

        public bool IsSaver(string name)
        {
            return Can(name) && PluginItems[name].IsSaver;
        }
        public bool IsSaver(RuleItem rule)
        {
            return IsSaver(rule.Name);
        }

        public IRule? Render(string name)
        {
            if (!PluginItems.TryGetValue(name, out PluginItem? item))
            {
                return null;
            }
            if (item == null || item.Callback == null)
            {
                return null;
            }
            var rule = item.Callback;
            var ctor = rule.GetConstructor(Array.Empty<Type>());
            if (ctor == null)
            {
                return null;
            }
            return ctor.Invoke(Array.Empty<object>()) as IRule;
        }

        public IRule? Render(RuleItem rule)
        {
            var instance = Render(rule.Name);
            if (instance == null)
            {
                return null;
            }
            instance.Ready(rule);
            return instance;
        }

        public IList<IRule> Render(IEnumerable<RuleItem> rules)
        {
            var shouldPrepare = false;
            return Render(rules, ref shouldPrepare);
        }
        public IList<IRule> Render(IEnumerable<RuleItem> rules, ref bool shouldPrepare)
        {
            var items = new List<IRule>();
            var booted = false;
            foreach (var rule in rules)
            {
                if (rule == null)
                {
                    continue;
                }
                var r = Render(rule);
                if (r == null)
                {

                    continue;
                }
                items.Add(r);
                if (!booted && (r is not IRuleSaver || (r as IRuleSaver)!.ShouldPrepare))
                {
                    // 只取第一个判断是否需要预载
                    shouldPrepare = true;
                    booted = true;
                }
                if (r is IRuleSaver && !(r as IRuleSaver)!.CanNext)
                {
                    break;
                }
            }
            return items;
        }


        public IList<PluginItem> All()
        {
            return PluginItems.Values.Cast<PluginItem>().ToList();
        }


        public void Load()
        {
            new Rules.Startup().Boot(this);
        }

        public void Load(string pluginDirectory)
        {
            Load();
            if (string.IsNullOrWhiteSpace(pluginDirectory))
            {
                return;
            }
            var info = new DirectoryInfo(pluginDirectory);
            if (!info.Exists)
            {
                return;
            }
            var files = info.GetFiles();
            foreach (var item in files)
            {
                if (item.Name.IndexOf("Rule.dll") < 0)
                {
                    continue;
                }
                LoadDll(item.FullName);
            }
            Debug.WriteLine($"成功加载{PluginItems.Count}条规则");
        }

        public void LoadPlugin(Type[] ruleMaps, string fileName)
        {
            foreach (var item in ruleMaps)
            {
                LoadPlugin(item, fileName);
            }
        }

        public void LoadPlugin(Type rule, string fileName)
        {
            AddRule(rule, fileName);
        }

        public void LoadDll(string path)
        {
            var loadContext = new PluginLoadContext(path);
            var assem = loadContext.LoadFromAssemblyName(AssemblyName.GetAssemblyName(path));
            if (assem == null)
            {
                return;
            }
            LoadPlugin(assem.GetTypes(), path);
        }

        public void AddRule(Assembly assemby)
        {
            LoadPlugin(assemby.GetTypes(), string.Empty);
        }

        public void AddRule(Type rule)
        {
            AddRule(rule, string.Empty);
        }

        public void AddRule(Type rule, string fileName)
        {
            if (rule == null || !typeof(IRule).IsAssignableFrom(rule))
            {
                return;
            }
            if (Activator.CreateInstance(rule) is not IRule instance)
            {
                return;
            }
            var info = instance.Info();
            var isSaver = instance is IRuleSaver;
            PluginItems.Add(info.Name, new PluginItem(info)
            {
                Callback = rule,
                FileName = fileName,
                ShouldPrepare = isSaver && (instance as IRuleSaver)!.ShouldPrepare,
                CanNext = isSaver && (instance as IRuleSaver)!.CanNext,
                IsSaver = isSaver
            });
        }

        public void AddRule(IEnumerable<Type> rules)
        {
            foreach (var item in rules)
            {
                AddRule(item);
            }
        }
    }
}
