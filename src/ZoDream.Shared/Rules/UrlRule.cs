﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZoDream.Shared.Interfaces;

namespace ZoDream.Shared.Rules
{
    public class UrlRule : IRule
    {
        public ISpider Container { get; set; }
        public IRuleValue Render(IRuleValue value)
        {
            throw new NotImplementedException();
        }
    }
}