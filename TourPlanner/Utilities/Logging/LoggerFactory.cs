using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TourPlanner.Utilities.Logging
{
    public static class LoggerFactory
    {
        public static ILoggerWrapper GetLogger()
        {
            StackTrace stackTrace = new(1, false);
            var type = stackTrace.GetFrame(1)?.GetMethod()?.DeclaringType;
            return Log4NetWrapper.CreateLogger("./log4net.config", type?.FullName ?? "");
        }
    }

}
