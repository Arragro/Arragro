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


        public void SendError(string errorText)
        {
            var message = new SG.SendGridMessage();
            message.From = new MailAddress("suuport@arragro.com", _applicationName);
            message.AddTo("support@arragro.com");

#if RELEASE
            message.AddTo("support@arragro.co.nz");
            message.Subject = string.Format("Something went wrong!");
#else
            message.AddTo("support@arragro.co.nz");
            message.Subject = string.Format("TEST - Something went wrong!");
#endif
            message.Text = errorText;
            message.Html = errorText;

            _transportWeb.DeliverAsync(message);
        }
    }
}
