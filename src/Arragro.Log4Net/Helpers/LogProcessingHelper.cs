using log4net;
using System;

namespace Arragro.Log4Net.Helpers
{
    public static class LogProcessingHelper
    {
        private static ILog _log;

        static LogProcessingHelper()
        {
            _log = LogManager.GetLogger(typeof(LogProcessingHelper));
        }

        public static T ExecuteAndLogTimeTaken<T>(Func<T> func, string message)
        {
            var start = DateTime.Now;
            var data = func.Invoke();
            _log.Info(string.Format("{0} executed and took: {1}", message, new TimeToComplete(start).ToString()));
            return data;
        }

        public static void ExecuteAndLogTimeTaken(Action action, string message)
        {
            var start = DateTime.Now;
            action.Invoke();
            _log.Info(string.Format("{0} executed and took: {1}", message, new TimeToComplete(start).ToString()));
        }

        public static T ExecuteAndLogTimeTaken<T>(this ILog log, Func<T> func, string message)
        {
            var start = DateTime.Now;
            var data = func.Invoke();
            log.Info(string.Format("{0} executed and took: {1}", message, new TimeToComplete(start).ToString()));
            return data;
        }

        public static void ExecuteAndLogTimeTaken(this ILog log, Action action, string message)
        {
            var start = DateTime.Now;
            action.Invoke();
            log.Info(string.Format("{0} executed and took: {1}", message, new TimeToComplete(start).ToString()));
        }
    }
}
