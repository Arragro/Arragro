using Arragro.Common.Logging;
using SendGrid.Helpers.Mail;
using System;
using System.Diagnostics;
using System.IO;
using System.Net.Mail;
using System.Threading.Tasks;
using SG = SendGrid;

namespace Arragro.SendGrid
{
    public class EmailHelper
    {
        private readonly ILog _log = LogManager.GetLogger("SendGrid.EmailHelper");
        private readonly TextWriter _twLog = null;
        private readonly SG.ISendGridClient _transportWeb;
        private readonly string _applicationName;
        private readonly bool _testMode;
        
        public EmailHelper(string applicationName, string sendgridApiKey, bool testMode = false)
        {
            _applicationName = applicationName;
            _testMode = testMode;
            _transportWeb = new SG.SendGridClient(sendgridApiKey);
        }

        public EmailHelper(string applicationName, string sendgridApiKey, TextWriter log, bool testMode = false) : this(applicationName, sendgridApiKey, testMode)
        {
            _twLog = log;
        }

        private void LogInfo(string message)
        {
            if (_twLog != null)
                _twLog.WriteLine(message);
            _log.Info(message);
        }

        public async Task SendEmail(
            string fromEmail, string toEmail, 
            string subject, string text, string html, 
            string displayName = null)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            var message = new SendGridMessage();
            message.From = new EmailAddress(fromEmail, displayName);
#if DEBUG || DEV
            message.AddTo("support@arragro.com");
            message.Subject = string.Format("{0} - {1} - Originally To: {2}", _applicationName, subject, toEmail);
#else

            if (_testMode)
            {
                message.AddTo("support@arragro.com");
                message.Subject = string.Format("{0} - {1} - Originally To: {2}", _applicationName, subject, toEmail);
            }
            else
            {
                message.AddTo(toEmail);
                message.Subject = subject;
            }
#endif

            message.PlainTextContent = text;
            message.HtmlContent = html;

            try
            {
                await _transportWeb.SendEmailAsync(message);
            }
            catch (Exception ex)
            {
                _log.Error(ex); 
            }

            stopwatch.Stop();
            LogInfo("SendEmail took " + stopwatch.ElapsedMilliseconds + "ms");
        }

        public async Task SendError(string errorText)
        {
            var message = new SendGridMessage();
            message.From = new EmailAddress("support@arragro.com", _applicationName);
            message.AddTo("support@arragro.com");

#if RELEASE
            await SendEmail(
                "support@arragro.com", "support@arragro.co.nz",
                "Something went wrong!", errorText, errorText,
                _applicationName);
#else
            await SendEmail(
                "support@arragro.com", "support@arragro.co.nz",
                "TEST - Something went wrong!", errorText, errorText,
                _applicationName);
#endif
        }

        public async Task SendError(Exception ex)
        {
            if (ex.ToString().Contains("Could not load file or assembly 'Microsoft.SqlServer.Types, Version=10.0.0.0"))
            {
                const string webConfigSqlServerTypes = @"
Please add the following to you web config in the runtime\assemblyBinding section:

<dependentAssembly>
    <assemblyIdentity name=""Microsoft.SqlServer.Types"" publicKeyToken=""89845dcd8080cc91"" culture=""neutral"" />
    <bindingRedirect oldVersion=""10.0.0.0-11.0.0.0"" newVersion=""11.0.0.0"" />
</dependentAssembly>";
                ex = new Exception(webConfigSqlServerTypes, ex);
            }

            await SendError(string.Format("<pre>{0}</pre>", ex.ToString()));
        }
    }
}
