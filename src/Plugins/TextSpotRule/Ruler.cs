using System;
using System.Threading.Tasks;
using ZoDream.Shared.Interfaces;
using ZoDream.Shared.Local;
using ZoDream.Shared.Models;

namespace ZoDream.Spider.TextSpotRule
{
    public class Ruler : IRule, IRuleSaver
    {
        private string tag = string.Empty;

        public bool ShouldPrepare => false;

        public bool CanNext => true;

        public string GetFileName(string url)
        {
            return string.Empty;
        }

        public PluginInfo Info()
        {
            return new PluginInfo("½ØÍ¼Ê¶±ð");
        }

        public void Ready(RuleItem option)
        {
            tag = option.Param1.Trim();
        }

        public void Render(ISpiderContainer container)
        {
            _ = RenderAsync(container);
        }

        private async Task RenderAsync(ISpiderContainer container)
        {
            var script = Open.Read(AppDomain.CurrentDomain.BaseDirectory + "\\html2canvas.js");

            var base64 = await container.Application.RequestProvider.Getter()
                .ExecuteScriptAsync(container.Url.Source, 
                script + ";var canvas = await html2canvas(document.querySelector('" + tag 
                + "'));return canvas.toDataURL('image/png')");
            // TODO 
        }
    }
}
