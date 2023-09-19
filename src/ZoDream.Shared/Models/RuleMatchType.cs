using System;
using System.Collections.Generic;
using System.Text;

namespace ZoDream.Shared.Models
{
    public enum RuleMatchType
    {
        None,
        All,
        Contains,
        Regex,
        Host,
        StartWith,
        Event,
        Page, // 单页包含资源
    }
}
