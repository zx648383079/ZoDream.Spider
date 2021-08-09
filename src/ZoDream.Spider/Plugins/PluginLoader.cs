using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using ZoDream.Shared.Interfaces;

namespace ZoDream.Spider.Plugins
{
    public class PluginLoader
    {

        public static void Loader(string path)
        {
            var assem = Assembly.LoadFile(path);
            if (assem == null)
            {
                return;
            }
            var type = assem.GetType("ZoDream.Spider.MatchRule.Loader");
            if (type == null)
            {
                return;
            }
            if (!type.IsInstanceOfType(typeof(ILoader)))
            {
                return;
            }
            var ctor = type.GetConstructor(new Type[] { typeof(ILoader) });
            if (ctor == null)
            {
                return;
            }
            var instance = ctor.Invoke(new object[] {});
            if (instance == null)
            {
                return;
            }
            var method = type.GetMethod("Serializer");
            if (method == null)
            {
                return;
            }
            var res = method.Invoke(instance, null);

        }
    }
}
