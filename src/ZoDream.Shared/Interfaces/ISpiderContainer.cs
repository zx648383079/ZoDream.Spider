using ZoDream.Shared.Models;

namespace ZoDream.Shared.Interfaces
{
    public interface ISpiderContainer
    {
        public ISpider Application { get; set; }

        public UriItem Url { get; set; }

        public IList<IRule> Rules { get; set; }
        public int RuleIndex { get; }

        public IRuleValue Data { get; set; }

        public IEnumerable<string> AttributeKeys { get; }
        /// <summary>
        /// 添加网址
        /// </summary>
        /// <param name="uri"></param>
        /// <returns>新的相对路径</returns>
        public string AddUri(string uri);

        public string AddUri(string uri, UriType uriType);

        public void SetAttribute(string name, string value);

        public void UnsetAttribute(string name);

        public string GetAttribute(string name);

        public void Next();

        public string RenderData(string content);
    }
}
