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
    public class EventLogger : ILogger
    {
        public EventLogger() : this(LogLevel.Debug)
        {

        }

        public EventLogger(LogLevel level)
        {
            Level = level;
        }
        public LogLevel Level { get; private set; }

        public event LogEventHandler? OnLog;
        public event ProgressEventHandler? OnProgress;

        public void Error(string message)
        {
            Log(LogLevel.Error, message);
        }

        public void Info(string message)
        {
            Log(LogLevel.Info, message);
        }

        public void Debug(string message)
        {
            Log(LogLevel.Debug, message);
        }

        public void Log(string message)
        {
            Log(Level, message);
        }

        public void Log(LogLevel level, string message)
        {
            if (level >= Level)
            {
                OnLog?.Invoke(message, level);
            }
            // Debug.WriteLine(message);
        }

        public void Progress(long current, long total)
        {
            Progress(current, total, string.Empty);
        }

        public void Waining(string message)
        {
            Log(LogLevel.Warn, message);
        }

        public void Progress(long current, long total, string message)
        {
            OnProgress?.Invoke(current, total, message);
        }

    }

    public delegate void LogEventHandler(string message, LogLevel level);
    public delegate void ProgressEventHandler(long current, long total, string message);
}
