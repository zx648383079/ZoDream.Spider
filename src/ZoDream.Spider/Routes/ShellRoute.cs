using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZoDream.Shared.Routes
{
    public class ShellRoute
    {
        public string Name { get; set; }

        public Type Page { get; set; }

        public Type? DataContext { get; set; }

        public ShellRoute(string name, Type page)
        {
            Name = name;
            Page = page;
        }

        public ShellRoute(string name, Type page, Type data): this(name, page)
        {
            DataContext = data;
        }
    }
}
