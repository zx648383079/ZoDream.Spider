using System.Collections.Generic;
using ZoDream.Shared.Models;

namespace ZoDream.Shared.Interfaces
{
    public interface IPluginLoader
    {

        public bool Can(string name);
        public bool Can(RuleItem rule);

        public bool IsUseLazy(string name);
        public bool IsUseLazy(RuleItem rule);
        public bool IsSaver(string name);
        public bool IsSaver(RuleItem rule);

        public IRule? Render(string name);
        public IRule? Render(RuleItem rule);
        public IList<IRule> Render(IEnumerable<RuleItem> rules);
        public IList<IRule> Render(IEnumerable<RuleItem> rules, ref bool shouldPrepare);

        public IList<PluginItem> All();
        public void Load(string pluginDirectory);
        public void Load();
    }
}
