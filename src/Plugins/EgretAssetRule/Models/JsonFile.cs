using System.Collections.Generic;

namespace ZoDream.Spider.EgretAssetRule.Models
{
    internal class JsonFile
    {
        public string Name { get; set; } = string.Empty;
        public string ImagePath { get; set; } = string.Empty;

        public int Width { get; set; }

        public int Height { get; set; }

        public IList<JsonSubItem>? SubTexture { get; set; }

    }

    internal class JsonSubItem
    {
        public string Name { get; set; } = string.Empty;

        public int X { get; set; }
        public int Y { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public int FrameX { get; set; }
        public int FrameY { get; set; }
        public int FrameWidth { get; set; }
        public int FrameHeight { get; set; }
    }
}
