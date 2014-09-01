using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Arragro.Common.Logging
{
    public class LogManager
    {
        private static ILogFactory _logFactory;

        public static ILogFactory LogFactory
        {
            get
            {
                if (_logFactory == null)
                    return new DebugLogManager();
                return _logFactory;
            }
            set
            {
                _logFactory = value;
            }
        }

        public static ILog GetLogger(Type type)
        {
            return LogFactory.GetLogger(type);
        }

        public static ILog GetLogger(string typeName)
        {
            return LogFactory.GetLogger(typeName);
        }
    }
}
