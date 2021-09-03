using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZoDream.Shared.Interfaces;

namespace ZoDream.Shared.Loggers
{
    public class FileLogger : ILogger
    {
        public LogLevel Level => LogLevel.Debug;

        public void Error(string message)
        {
            throw new NotImplementedException();
        }

        public void Info(string message)
        {
            throw new NotImplementedException();
        }

        public void Log(string message)
        {
            throw new NotImplementedException();
        }

        public void Log(IRule rule)
        {
            throw new NotImplementedException();
        }

        public void Log(IRule rule, ISpiderContainer container)
        {
            throw new NotImplementedException();
        }

        public void Waining(string message)
        {
            throw new NotImplementedException();
        }

        public void Log(string message)
        {
            if (!IsDebug)
            {
                return;
            }
            Debug.WriteLine(message);
        }
        public void Log(IRule rule)
        {
            if (!IsDebug)
            {
                return;
            }
            var info = rule.Info();
            Log($"进入规则：{info.Name}");
        }
        public void Log(IRule rule, ISpiderContainer container)
        {
            if (!IsDebug)
            {
                return;
            }
            var info = rule.Info();
            Log($"执行规则：{info.Name}");
            var i = -1;
            foreach (var item in container.Data)
            {
                i++;
                Log($"[{i}]: {item}");
            }
        }
    }
}
