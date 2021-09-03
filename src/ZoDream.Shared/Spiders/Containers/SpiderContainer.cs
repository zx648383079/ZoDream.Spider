using ZoDream.Shared.Interfaces;
using ZoDream.Shared.Models;

namespace ZoDream.Shared.Spiders.Containers
{
    public class SpiderContainer : ISpiderContainer
    {
        public SpiderContainer(ISpider spider)
        {
            Application = spider;
        }
        public ISpider Application { get; set; }

        public IDictionary<string, string> MapItems = new Dictionary<string, string>();
        public IList<IRule> Rules { get; set; }
        public IRuleValue Data { get; set; }
        public UriItem Url { get; set; }

        private int ruleIndex = -1;

        public string AddUri(string uri, UriType uriType)
        {
            var fromUri = new Uri(Url.Source);
            var toUri = new Uri(fromUri, uri);
            var fullUri = toUri.ToString();
            Application.UrlProvider.Add(fullUri, uriType);
            var saveFileName = Application.RuleProvider.GetFileName(uri);
            if (!string.IsNullOrEmpty(saveFileName))
            {
                return Path.GetRelativePath(Application.Option.WorkFolder, saveFileName);
            }
            var relativeUri = fromUri.MakeRelativeUri(toUri);
            return Uri.UnescapeDataString(relativeUri.ToString());
        }

        public void Next()
        {
            ruleIndex++;
            if (ruleIndex >= Rules.Count)
            {
                return;
            }
            var rule = Rules[ruleIndex];
            rule.Render(this);
        }

        public void SetAttribute(string name, string value)
        {
            MapItems.Add(name, value);
        }

        public void UnsetAttribute(string name)
        {
            MapItems.Remove(name);
        }

        public string AddUri(string uri)
        {
            return AddUri(uri, UriType.Html);
        }

        public string RenderData(string content)
        {
            throw new NotImplementedException();
        }
    }
}
