using Newtonsoft.Json;
using System.Threading.Tasks;
using ZoDream.Shared.Form;
using ZoDream.Shared.Interfaces;
using ZoDream.Shared.Models;
using ZoDream.Shared.Utils;
using ZoDream.Spider.EgretAssetRule.Models;

namespace ZoDream.Spider.EgretAssetRule
{
    public class Ruler : IRule, IRuleSaver
    {
        private bool SplitFrame;
        public bool ShouldPrepare => false;

        public bool CanNext => false;

        public IFormInput[]? Form()
        {
            return [
                Input.Switch(nameof(SplitFrame), "拆分CssSprite")
            ];
        }

        public string GetFileName(string url)
        {
            return Disk.RenderFile(url);
        }

        public PluginInfo Info()
        {
            return new("Egret资源下载");
        }

        public void Ready(RuleItem option)
        {
            SplitFrame = option.Get<bool>(nameof(SplitFrame));
        }

        public async Task RenderAsync(ISpiderContainer container)
        {
            var file = container.Application.Storage.GetAbsolutePath(GetFileName(container.Url.Source));
            if (!container.Url.Source.EndsWith(".json"))
            {
                await container.GetAsync(
                    file,
                    container.Url.Source);
                return;
            }
            var content = await container.GetAsync(container.Url.Source);
            if (string.IsNullOrWhiteSpace(content))
            {
                return;
            }
            if (content.Contains("\"resources\""))
            {
                await RenderResAsync(container, content);
            }
            else if (content.Contains("\"imagePath\""))
            {
                await RenderTexAsync(container, content);
            } 
            else if (content.Contains("\"frames\""))
            {
                await RenderSheetAsync(container, content);
            }
            await container.SaveAsync(file, content);
        }

        private async Task RenderResAsync(ISpiderContainer container, string content)
        {
            var data = JsonConvert.DeserializeObject<ResMap>(content);
            if (data?.Resources is null)
            {
                return;
            }
            foreach (var item in data.Resources)
            {
                switch (item.Type)
                {
                    case "json":
                        container.AddUri(item.Url, UriType.Json);
                        if (item.Url.EndsWith("_tex.json"))
                        {
                            container.AddUri(item.Url.Replace("_tex.json", "_ske.json"), UriType.Json);
                        }
                        break;
                    case "sheet":
                        container.AddUri(item.Url, UriType.Json);
                        break;
                    default:
                        container.AddUri(item.Url, UriType.File);
                        break;
                }
            }
        }
        private async Task RenderTexAsync(ISpiderContainer container, string content)
        {
            var data = JsonConvert.DeserializeObject<JsonFile>(content);
            if (data is null)
            {
                return;
            }
            container.AddUri(data.ImagePath, UriType.File);
        }
        private async Task RenderSheetAsync(ISpiderContainer container, string content)
        {
            var data = JsonConvert.DeserializeObject<FrameSheetFile>(content);
            if (data is null)
            {
                return;
            }
            container.AddUri(data.File, UriType.File);
        }
    }
}
