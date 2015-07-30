using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logging
{
    interface ILogger
    {
        public void Log(LogLevel level, string message);
    }
}
