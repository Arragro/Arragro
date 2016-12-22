using Arragro.Common.Logging;
using System;

namespace Arragro.Common.Exceptions
{
    public class SqlConnectivityException : Exception
    {
        public SqlConnectivityException(Exception ex, ILog log) : base("Sql connectivity issue", ex)
        {
            log.Error(ex.Message);
        }
    }
}
