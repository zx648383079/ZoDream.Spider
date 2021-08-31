﻿using ZoDream.Shared.Models;

namespace ZoDream.Shared.Interfaces
{
    public interface ISpiderContainer
    {
        public ISpider Application { get; set; }

        public UriItem Url { get; set; }

        public IList<RuleItem> Rules { get; set; }

        public IRuleValue Data { get; set; }
        /// <summary>
        /// 添加网址
        /// </summary>
        /// <param name="uri"></param>
        /// <returns>新的相对路径</returns>
        public string AddUri(string uri);

        public string AddUri(string uri, UriType uriType);

        public void SetAttribute(string name, string value);

        public void UnsetAttribute(string name);

        public void Next();
    }
}
