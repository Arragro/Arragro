using System;
using System.Configuration;

namespace Arragro.SendGrid
{
    public static class SendGridConfiguration
    {
        private static string _sendGridApiKey = null;
        public static string SendGridApiKey()
        {
            if (_sendGridApiKey == null)
            {
                if (ConfigurationManager.AppSettings["SendGridApiKey"] == null)
                    throw new ArgumentException("A valid SendGridApiKey needs to be configured in the appsettings section of your App.config.");
                _sendGridApiKey = ConfigurationManager.AppSettings["SendGridApiKey"].ToString();
            }
            return _sendGridApiKey;
        }

        private static bool? _testMode = null;
        public static bool TestMode()
        {
            if (!_testMode.HasValue)
            {
                if (ConfigurationManager.AppSettings["TestMode"] == null)
                    throw new ArgumentException("A valid TestMode needs to be configured in the appsettings section of your App.config.");
                bool testMode;
                if (bool.TryParse(ConfigurationManager.AppSettings["TestMode"].ToString(), out testMode))
                    _testMode = testMode;
                else
                    _testMode = true;
                return _testMode.Value;
            }
            return _testMode.Value;
        }
    }
}
