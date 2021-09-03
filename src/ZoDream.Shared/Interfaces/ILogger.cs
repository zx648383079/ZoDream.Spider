using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZoDream.Shared.Loggers;

namespace ZoDream.Shared.Interfaces
{
    public interface ILogger
    {
        public LogLevel Level { get;}

        public void Log(string message);
        public void Log(IRule rule);
        public void Log(IRule rule, ISpiderContainer container);

        public void Info(string message);

        public void Waining(string message);

        public void Error(string message);
    }
}
