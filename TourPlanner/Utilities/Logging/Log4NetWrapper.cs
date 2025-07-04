using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TourPlanner.Utilities.Logging
{
    public class Log4NetWrapper : ILoggerWrapper
    {
        private readonly log4net.ILog logger;

        public static Log4NetWrapper CreateLogger(string configPath, string caller)
        {
            if (!File.Exists(configPath))
            {
                throw new ArgumentException("Does not exist.", nameof(configPath));
            }

            log4net.Config.XmlConfigurator.Configure(new FileInfo(configPath));
            var logger = log4net.LogManager.GetLogger(caller);  
            return new Log4NetWrapper(logger);
        }

        private Log4NetWrapper(log4net.ILog logger)
        {
            this.logger = logger;
        }

        public void Debug(string message)
        {
            logger.Debug(message);
        }
        public void Warn(string message)
        {
            logger.Warn(message);
        }

        public void Error(string message)
        {
            logger.Error(message);
        }

        public void Fatal(string message)
        {
            logger.Fatal(message);
        }

        public void Info(string message)
        {
            logger.Info(message);
        }
    }


}
