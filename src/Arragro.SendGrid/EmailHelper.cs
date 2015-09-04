using Arragro.Common.Logging;
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
        private readonly SG.Web _transportWeb;
        private readonly string _applicationName;

        private EmailHelper()
        {
            _transportWeb = new SG.Web(SendGridConfiguration.SendGridApiKey());
        }

        public EmailHelper(string applicationName) : this()
        {
            _applicationName = applicationName;
        }

        public EmailHelper(string applicationName, TextWriter log) : this(applicationName)
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

            var message = new SG.SendGridMessage();
            message.From = new MailAddress(fromEmail, displayName);
#if DEBUG || DEV
            message.AddTo("support@arragro.com");
            message.Subject = string.Format("{0} - {1} - Originally To: {2}", _applicationName, subject, toEmail);
#else

            if (SendGridConfiguration.TestMode())
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

            message.Text = text;
            message.Html = html;

            try
            {
                await _transportWeb.DeliverAsync(message);
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
            var message = new SG.SendGridMessage();
            message.From = new MailAddress("support@arragro.com", _applicationName);
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
