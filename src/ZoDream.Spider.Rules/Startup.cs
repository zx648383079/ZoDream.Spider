using System.Reflection;
using ZoDream.Shared.Interfaces;

namespace ZoDream.Spider.Rules
{
    public class Startup
    {
        public void Boot(IServiceCollection service)
        {
            service.AddRule(Assembly.GetExecutingAssembly());
        }
    }
}
