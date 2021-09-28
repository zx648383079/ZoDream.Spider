using System;
using System.IO;
using System.Threading.Tasks;
using ZoDream.Shared.Interfaces;
using ZoDream.Shared.Local;
using ZoDream.Shared.Models;
using ZoDream.Shared.Utils;

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
            return new PluginInfo("��ͼʶ��");
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
                script + ";html2canvas(document.querySelector('" + tag 
                + "')).then(function(canvas) {zreSpider.Callback(canvas.toDataURL('image/png'))});"
            );
            if (base64 == null)
            {
                return;
            }
            if (base64.StartsWith("data:image/png;base64,"))
            {
                base64 = base64.Substring(22);
            }
            var data = Convert.FromBase64String(base64);
            var path = container.Application.GetAbsoluteFile(Disk.RenderFile(container.Url.Source));
            Disk.CreateDirectory(path);
            File.WriteAllBytes(path + ".png", data);
        }
    }
}
