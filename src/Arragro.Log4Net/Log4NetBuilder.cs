using Arragro.Log4Net.CSV;
using log4net;
using log4net.Appender;
using log4net.Core;
using log4net.Repository.Hierarchy;

namespace Arragro.Log4Net
{
    public class Log4NetBuilder
    {
        private static RollingFileAppender ConfigureAppender(
            string appPath,
            string fileName,
            Level levelMin,
            Level levelMax)
        {
            var patternLayout = new CsvPatternLayout();
            patternLayout.Header = "DateTime,Thread,Level,Logger,Message\r\n";
            patternLayout.ConversionPattern = "date{M/d/yyyy H:mm:ss.fff}%newfield[%thread]%newfield%level%newfield%logger%newfield%message%endrow";
            patternLayout.ActivateOptions();

            var roller = new RollingFileAppender();
            roller.File = $@"{appPath}\{fileName}";
            roller.AppendToFile = true;
            roller.RollingStyle = RollingFileAppender.RollingMode.Size;
            roller.MaxSizeRollBackups = 5;
            roller.MaximumFileSize = "10MB";
            roller.StaticLogFileName = true;
            roller.Layout = patternLayout;
            roller.AddFilter(new log4net.Filter.LevelRangeFilter { LevelMin = levelMin, LevelMax = levelMax });
            roller.LockingModel = new FileAppender.MinimalLock();
            roller.ActivateOptions();

            return roller;
        }

        public static void Setup(string appPath)
        {
            var hierarchy = (Hierarchy)LogManager.GetRepository();

            hierarchy.Root.AddAppender(ConfigureAppender(appPath, "log_info.csv", Level.Info, Level.Info));
            hierarchy.Root.AddAppender(ConfigureAppender(appPath, "log_warning.csv", Level.Warn, Level.Warn));
            hierarchy.Root.AddAppender(ConfigureAppender(appPath, "log_error.csv", Level.Error, Level.Fatal));
            hierarchy.Root.AddAppender(ConfigureAppender(appPath, "log_debug.csv", Level.Debug, Level.Debug));

            //MemoryAppender memory = new MemoryAppender();
            //memory.ActivateOptions();
            //hierarchy.Root.AddAppender(memory);

            hierarchy.Configured = true;
        }

        public static void Setup(string appPath, IAppender[] appenders)
        {
            var hierarchy = (Hierarchy)LogManager.GetRepository();

            foreach (var appender in appenders)
            {
                hierarchy.Root.AddAppender(appender);
            }

            hierarchy.Configured = true;
        }
    }
}
