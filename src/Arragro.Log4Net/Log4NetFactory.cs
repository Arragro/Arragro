using Arragro.Common.Logging;
using System;
using System.IO;

namespace Arragro.Log4Net
{
    public class Log4NetFactory : ILogFactory
    {
        public Log4NetFactory() : this(false) { }
        
        public Log4NetFactory(bool configureLog4Net)
        {
            if (configureLog4Net)
                // Configure log4net via the config file
                log4net.Config.XmlConfigurator.Configure();
        }

        public ILog GetLogger(Type type)
        {
            return new Log4NetLogger(type);
        }

        public ILog GetLogger(string typeName)
        {
            return new Log4NetLogger(typeName);
        }
    }
}
