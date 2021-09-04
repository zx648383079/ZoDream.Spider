﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZoDream.Shared.Models;

namespace ZoDream.Shared.Interfaces
{
    public interface IRuleProvider: ILoader
    {
        public IList<RuleGroupItem> Get(string uri);

        public IRule? Render(string name);
        public IRule? Render(RuleItem rule);
        public void Add(RuleGroupItem rule);
        public void Add(IList<RuleGroupItem> rules);
        public IList<RuleGroupItem> All();
        public string GetFileName(string uri);
        public IList<PluginItem> AllPlugin();
        public void Load(string pluginDirectory);
        public void Load();
    }
}
