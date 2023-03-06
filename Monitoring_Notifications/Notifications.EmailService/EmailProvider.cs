using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Logging;
using MimeKit;
using MimeKit.Text;
using Notifications.Options;

namespace Notifications.EmailService
{
    public class EmailProvider : IEmailProvider
    {
        private readonly Configs _configs;
        private readonly ILogger _logger;
        public EmailProvider(ILogger<EmailProvider> logger, Configs configs)
        {
            _configs = configs;
            _logger = logger;
        }

        public void Send(EmailConfig emailConfig, string tos, string subject, 
                            string html)
        {
            _logger.LogInformation("Sender : " + emailConfig.FromEmail + "; Receiver : " + tos);
            // create message
           // tos = "tan.le@truecommerce.com";
            var email = new MimeMessage();
            email.Subject = subject;
            email.Body = new TextPart(TextFormat.Html) { Text = html };
            email.From.Add(MailboxAddress.Parse(emailConfig.FromEmail));

            string[] emailAddresses = tos.Split(';');
            foreach (string emailAddress in emailAddresses)
            {
                email.To.Add(MailboxAddress.Parse(emailAddress));
            }

            int tryAgain = _configs.AppSettings.EmailRetryCount;
            int milliseconds = _configs.AppSettings.EmailDelayRetryMiliSeconds;
            bool failed = false;
            do
            {
                try
                {
                    failed = false;
                    // send email
                    using (var smtp = new SmtpClient())
                    {
                        if ((bool)(emailConfig == null ? false : emailConfig.EnableSSL))
                        {
                            smtp.Connect(emailConfig == null ? "" : emailConfig.SmtpServer,
                                (int)(emailConfig == null ? 0 : emailConfig.SmtpPort), SecureSocketOptions.StartTls);
                        }
                        else
                        {
                            smtp.Connect(emailConfig == null ? "" : emailConfig.SmtpServer,
                                (int)(emailConfig == null ? 0 : emailConfig.SmtpPort), SecureSocketOptions.None);
                        }
                        smtp.Authenticate(emailConfig == null ? "" : emailConfig.SmtpUser,
                                        emailConfig == null ? "" : emailConfig.SmtpPassword);
                        smtp.Send(email);
                        smtp.Disconnect(true);
                    }
                }
                catch (Exception ex)
                {
                    failed = true;
                    tryAgain--;
                    _logger.LogError(ex.Message, ex);
                    Task.Delay(milliseconds).ConfigureAwait(false);
                }
            }
            while (failed && tryAgain != 0);
            if (failed && tryAgain == 0)
            {
                throw new Exception("Sending email is incompleted. Look at log file for further info");
            }
        }
    }
}
