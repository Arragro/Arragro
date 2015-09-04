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
    }
}
