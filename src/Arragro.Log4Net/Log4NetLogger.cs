using Arragro.Common.Logging;
using log4net.Repository;
using System;

namespace Arragro.Log4Net
{
    public class Log4NetLogger : ILog
    {
        private readonly log4net.ILog _log;
        
        public Log4NetLogger(string typeName)
        {
            _log = log4net.LogManager.GetLogger(typeName);
        }

        public Log4NetLogger(Type type)
        {
            _log = log4net.LogManager.GetLogger(type);
        }

        public bool IsDebugEnabled { get { return _log.IsDebugEnabled; } }

        public void Debug(object message)
        {
            if (_log.IsDebugEnabled)
                _log.Debug(message);
        }

        public void Debug(object message, Exception exception)
        {
            if (_log.IsDebugEnabled)
                _log.Debug(message, exception);
        }

        public void DebugFormat(string format, params object[] args)
        {
            if (_log.IsDebugEnabled)
                _log.DebugFormat(format, args);
        }

        public void Info(object message)
        {
            if (_log.IsInfoEnabled)
                _log.Info(message);
        }

        public void Info(object message, Exception exception)
        {
            if (_log.IsInfoEnabled)
                _log.Info(message, exception);
        }

        public void InfoFormat(string format, params object[] args)
        {
            if (_log.IsInfoEnabled)
                _log.InfoFormat(format, args);
        }

        public void Warn(object message)
        {
            if (_log.IsWarnEnabled)
                _log.Warn(message);
        }

        public void Warn(object message, Exception exception)
        {
            if (_log.IsWarnEnabled)
                _log.Warn(message, exception);
        }

        public void WarnFormat(string format, params object[] args)
        {
            if (_log.IsWarnEnabled)
                _log.WarnFormat(format, args);
        }

        public void Error(object message)
        {
            if (_log.IsErrorEnabled)
                _log.Error(message);
        }

        public void Error(object message, Exception exception)
        {
            if (_log.IsErrorEnabled)
                _log.Error(message, exception);
        }

        public void ErrorFormat(string format, params object[] args)
        {
            if (_log.IsErrorEnabled)
                _log.ErrorFormat(format, args);
        }

        public void Fatal(object message)
        {
            if (_log.IsFatalEnabled)
                _log.Fatal(message);
        }

        public void Fatal(object message, Exception exception)
        {
            if (_log.IsFatalEnabled)
                _log.Fatal(message, exception);
        }

        public void FatalFormat(string format, params object[] args)
        {
            if (_log.IsFatalEnabled)
                _log.FatalFormat(format, args);
        }
    }
}
