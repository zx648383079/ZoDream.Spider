using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ZoDream.Shared.Interfaces;
using ZoDream.Shared.Loggers;

namespace ZoDream.Spider.Loggers
{
    public class FileLogger : ILogger
    {
        public LogLevel Level => LogLevel.Debug;

        public FileLogger()
        {

        }

        public void Error(string message)
        {
            Log(LogLevel.Error, message);
        }

        public void Debug(string message)
        {
            Log(LogLevel.Debug, message);
        }

        public void Info(string message)
        {
            Log(LogLevel.Info, message);
        }


        public void Waining(string message)
        {
            Log(LogLevel.Warn, message);
        }

        public void Log(string message)
        {
            if (Level != LogLevel.Debug)
            {
                return;
            }
            Log(Level, message);
        }
        public void Log(IRule rule)
        {
            if (Level != LogLevel.Debug)
            {
                return;
            }
            var info = rule.Info();
            Log($"进入规则：{info.Name}");
        }
        public void Log(IRule rule, ISpiderContainer container)
        {
            if (Level != LogLevel.Debug)
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

        public void Log(LogLevel level, string message)
        {
            if (level < Level)
            {
                return;
            }

        }

        public void Progress(long current, long total)
        {
            Progress(current, total, string.Empty);
        }

        public void Progress(long current, long total, string message)
        {
            
        }

    }
}
