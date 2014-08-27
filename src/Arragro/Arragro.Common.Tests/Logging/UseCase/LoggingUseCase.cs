using Arragro.Common.Logging;
using System.Diagnostics;
using Xunit;

namespace Arragro.Common.Tests.Logging.UseCase
{
    public class LoggingUseCase
    {
        [Fact]
        public void TestDebugFactory()
        {
            Debug.AutoFlush = true;
            LogManager.LogFactory = new DebugLogManager();
            var logger = LogManager.GetLogger("TestLogger");
            logger.Debug("Hello");
            logger.DebugFormat("Foo {0}", "Bar");
        }
    }
}
