using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Logging
{
    public class LoggerFactory
    {
        private static ILogger instance;

        public static ILogger GetLogger()
        {
            if (instance == null)
            {
                instance = new Log4NetLogger();
            }
            return instance;
        }
    }
}
