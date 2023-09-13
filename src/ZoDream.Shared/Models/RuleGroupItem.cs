﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using ZoDream.Shared.Utils;

namespace ZoDream.Shared.Models
{
    public class RuleGroupItem
    {
        public RuleMatchType MatchType { get; set; } = RuleMatchType.All;

        public string MatchValue { get; set; } = string.Empty;

        public List<RuleItem> Rules { get; set; } = new();

        public bool IsMatch(string uri)
        {
            switch (MatchType)
            {
                case RuleMatchType.All:
                    return true;
                case RuleMatchType.Contains:
                    return uri.Contains(MatchValue);
                case RuleMatchType.Regex:
                    try
                    {
                        return Regex.IsMatch(uri, MatchValue);
                    }
                    catch (Exception)
                    {
                        return false;
                    }
                case RuleMatchType.Host:
                    return Html.MatchHost(uri) == MatchValue;
                case RuleMatchType.StartWith:
                    return uri.StartsWith(MatchValue);
                case RuleMatchType.Event:
                default:
                    return false;
            }
        }
    }
}
