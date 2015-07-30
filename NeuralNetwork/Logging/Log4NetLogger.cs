using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logging
{
    public class Log4NetLogger : ILogger
    {
        private readonly ILog _loggerImpl;

        public void Log(LogLevel level, string message)
        {
            switch (level)
            {
                case LogLevel.Debug:
                    _loggerImpl.Debug(message);
                    break;
                case LogLevel.Info:
                    _loggerImpl.Info(message);
                    break;
                case LogLevel.Warning:
                    _loggerImpl.Warn(message);
                    break;
                case LogLevel.Error:
                    _loggerImpl.Error(message);
                    break;
                case LogLevel.Fatal:
                    _loggerImpl.Fatal(message);
                    break;
            }
        }
    }
}
