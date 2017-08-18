namespace Arragro.Common.Interfaces.Providers
{
    public interface IEmailProcessor
    {
        void SendEmail(string subject, string text, string html, string from, string to);
        void SendEmail(string subject, string text, string html, string to);
    }
}
