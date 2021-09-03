using JiebaNet.Segmenter;
using System;
using ZoDream.Shared.Interfaces;
using ZoDream.Shared.Models;

namespace ZoDream.Spider.ImportSearchRule
{
    public class Ruler : IRule
    {
        private string apiUri = string.Empty;
        private string apiToken = string.Empty;
        public PluginInfo Info()
        {
            return new PluginInfo("µ¼ÈëËÑË÷ÒýÇæ");
        }

        public void Ready(RuleItem option)
        {
            apiUri = option.Param1.Trim();
        }

        public void Render(ISpiderContainer container)
        {
            var segmenter = new JiebaNet.Analyser.TfidfExtractor();
            var tags = segmenter.ExtractTags(container.Data.ToString());
        }
    }
}
