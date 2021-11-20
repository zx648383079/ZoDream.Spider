using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ZoDream.Shared.Utils
{
    public static class Str
    {

        public static int ToInt(string v)
        {
            if (string.IsNullOrWhiteSpace(v))
            {
                return 0;
            }
            if (int.TryParse(v, out var res))
            {
                return res;
            }
            return 0;
        }
    }
}
