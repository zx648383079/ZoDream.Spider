using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZoDream.Shared.Interfaces;
using ZoDream.Shared.Models;

namespace ZoDream.Shared.Spiders.Containers
{
    public class SpiderContainer : ISpiderContainer
    {
        public ISpider Application { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public string Url { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public IList<RuleItem> Rules { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public IRuleValue Data { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public void AddUri(string uri)
        {
            throw new NotImplementedException();
        }

        public void Next()
        {
            throw new NotImplementedException();
        }

        public void SetAttribute(string name, string value)
        {
            throw new NotImplementedException();
        }

        public void UnsetAttribute(string name)
        {
            throw new NotImplementedException();
        }
    }
}
