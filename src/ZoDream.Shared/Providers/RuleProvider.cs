using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using ZoDream.Shared.Interfaces;
using ZoDream.Shared.Models;
using ZoDream.Shared.Rules;

namespace ZoDream.Shared.Providers
{
    public class RuleProvider : IRuleProvider
    {
        public IList<RuleGroupItem> Items { get; private set; } = new List<RuleGroupItem>();

        internal IDictionary<string, PluginItem> PluginItems = new Dictionary<string, PluginItem>();

        public IList<RuleGroupItem> Get(string uri)
        {
            return Items.Where(item => item.IsMatch(uri)).ToList();
        }

        public void Add(RuleGroupItem rule)
        {
            Items.Add(rule);
        }

        public void Add(IList<RuleGroupItem> rules)
        {
            Items = rules.ToList();
        }
        public IList<RuleGroupItem> All()
        {
            return Items.ToList();
        }

        public IRule? Render(string name)
        {
            var item = PluginItems[name];
            if (item == null || item.Callback == null)
            {
                return null;
            }
            var rule = item.Callback;
            var ctor = rule.GetConstructor(new Type[] { typeof(IRule) });
            if (ctor == null)
            {
                return null;
            }
            return ctor.Invoke(new object[] { }) as IRule;
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

        public string GetFileName(string uri)
        {
            var items = Get(uri);
            foreach (var item in items)
            {
                foreach (var it in item.Rules)
                {
                    var rule = Render(it);
                    if (rule is IRuleSaver)
                    {
                        return (rule as IRuleSaver).GetFileName(uri);
                    }
                }
            }
            return string.Empty;
        }

        public void Deserializer(StreamReader reader)
        {
            var sb = new StringBuilder();
            string? line;
            while (null != (line = reader.ReadLine()))
            {
                if (string.IsNullOrWhiteSpace(line))
                {
                    break;
                }
                sb.AppendLine(line);
            }
            if (sb.Length < 1)
            {
                return;
            }
            Items = JsonConvert.DeserializeObject<IList<RuleGroupItem>>(sb.ToString());
        }

        public void Serializer(StreamWriter writer)
        {
            writer.WriteLine(JsonConvert.SerializeObject(Items));
        }

        public void Load()
        {
            var ruleMaps = new Type[] {
                typeof(DownloadRule),
                typeof(HtmlToTextRule),
                typeof(HtmlUrlRule),
                typeof(JoinRule),
                typeof(JQueryRule),
                typeof(MatchRule),
                typeof(NarrowRule),
                typeof(RegexReplaceRule),
                typeof(RegexRule),
                typeof(ReplaceRule),
                typeof(RestRule),
                typeof(SaveRule),
                typeof(TraditionalToSimplifiedRule),
                typeof(UrlRule),
                typeof(XPathRule),
            };
            LoadPlugin(ruleMaps, string.Empty);
        }

        private void LoadPlugin(Type[] ruleMaps, string fileName)
        {
            foreach (var item in ruleMaps)
            {
                LoadPlugin(item, fileName);
            }
        }

        private void LoadPlugin(Type rule, string fileName)
        {
            if (rule == null || !rule.IsInstanceOfType(typeof(IRule)))
            {
                return;
            }
            var ctor = rule.GetConstructor(new Type[] { typeof(IRule) });
            if (ctor == null)
            {
                return;
            }
            var instance = ctor.Invoke(new object[] { }) as IRule;
            if (instance == null)
            {
                return;
            }
            var info = instance.Info();
            PluginItems.Add(info.Name, new PluginItem(info)
            {
                Callback = rule,
                FileName = fileName
            });
        }

        public void LoadDll(string path)
        {
            var assem = Assembly.LoadFile(path);
            if (assem == null)
            {
                return;
            }
            LoadPlugin(assem.GetTypes(), path);
        }
    }
}
