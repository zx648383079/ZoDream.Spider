using System;
using System.Threading.Tasks;
using ZoDream.Shared.Form;
using ZoDream.Shared.Interfaces;
using ZoDream.Shared.Models;
using ZoDream.Shared.Storage;
using ZoDream.Shared.Utils;

namespace ZoDream.Spider.TextSpotRule
{
    public class Ruler : IRule, IRuleSaver
    {
        private string Tag = string.Empty;

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

        public IFormInput[]? Form()
        {
            return new IFormInput[] { Input.Text(nameof(Tag), "Ñ¡ÔñÆ÷", true) };
        }

        public void Ready(RuleItem option)
        {
            Tag = option.Get<string>(nameof(Tag)) ?? string.Empty;
        }

        public async Task RenderAsync(ISpiderContainer container)
        {
            var script = await LocationStorage.ReadAsync(AppDomain.CurrentDomain.BaseDirectory + "\\html2canvas.js");
            var base64 = await container.Application.RequestProvider.Getter()
                .ExecuteScriptAsync(container.Url.Source,
                script + ";html2canvas(document.querySelector('" + Tag 
                + "')).then(function(canvas) {zreSpider.Callback(canvas.toDataURL('image/png'))});"
            );
            if (base64 == null)
            {
                return;
            }
            if (base64.StartsWith("data:image/png;base64,"))
            {
                base64 = base64[22..];
            }
            var data = Convert.FromBase64String(base64);
            var file = Disk.RenderFile(container.Url.Source) + ".png";
            await container.Application.Storage.CreateAsync(file, data);
        }
    }
}
