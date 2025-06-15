using Microsoft.Extensions.Options;
using MimeKit;
using System.Net.Mail;

namespace Firebase.Authentication.Helpers
{
    public static class EmailHelper
    {
        private static EmailConfig _emailConfig;

        public static void AppSettingConfig(IOptions<EmailConfig> config)
        {
            _emailConfig = config.Value;
        }

        public static async Task SendEmailAsync(string email, string subject, string message, List<string>? attachments = null)
        {

            bool valid = true;
            try
            {
                MailAddress? emailAddress = new MailAddress(email);
            }
            catch
            {
                valid = false;
            }
            string to = valid ? email : throw new Exception("Email is not valid.");

            MimeMessage? emailToSend = new MimeMessage();
            emailToSend.From.Add(MailboxAddress.Parse(_emailConfig.From));

            emailToSend.To.Add(MailboxAddress.Parse(to));
            emailToSend.Subject = subject;

            BodyBuilder builder = new BodyBuilder();
            builder.HtmlBody = message;

            if (attachments != null)
            {
                foreach (string attachment in attachments)
                {
                    MemoryStream memoryStream = new MemoryStream(File.ReadAllBytes(attachment));
                    builder.Attachments.Add(attachment, memoryStream);
                }
            }

            emailToSend.Body = builder.ToMessageBody();

            using var emailClient = new MailKit.Net.Smtp.SmtpClient();
            emailClient.Connect(_emailConfig.Server, _emailConfig.Port, MailKit.Security.SecureSocketOptions.StartTls);
            await emailClient.AuthenticateAsync(_emailConfig.AuthEmail, _emailConfig.AuthPass);
            await emailClient.SendAsync(emailToSend);
            emailClient.Disconnect(true);
        }
    }
}
