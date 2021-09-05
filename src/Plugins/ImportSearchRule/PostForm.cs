using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZoDream.Spider.ImportSearchRule
{
    public class PostForm
    {
        public string Title { get; set; }

        public string Description { get; set; }

        public string Content { get; set; }

        public string Link { get; set; }

        public IEnumerable<string> Keywords { get; set; }
    }
}
