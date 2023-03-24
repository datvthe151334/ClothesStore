using ClothesStoreAPI.Template;
using System.Net;
using System.Net.Mail;

namespace ClothesStoreAPI.Configuration
{
    public class MailConfiguration
    {
        public static bool SendRecoveryMail(string email, string password, IConfiguration configuration)
        {
            bool isSend = false;
            try
            {
                using (MailMessage mail = new MailMessage())
                {
                    SmtpClient SmtpServer = new SmtpClient();
                    mail.From = new MailAddress(configuration.GetValue<string>("Smtp:FromAddress"));
                    mail.To.Add(email);
                    mail.Subject = "Password Recovery";
                    mail.Body = string
                        .Format(EmailTemplate.RECOVERY_EMAIL_TEMPLATE, email, password);
                    mail.IsBodyHtml = true;
                    SmtpServer.UseDefaultCredentials = false;
                    NetworkCredential NetworkCred = new NetworkCredential(
                        configuration.GetValue<string>("Smtp:UserName"),
                        configuration.GetValue<string>("Smtp:Password")
                        );
                    SmtpServer.Credentials = NetworkCred;
                    SmtpServer.EnableSsl = true;
                    SmtpServer.Port = configuration.GetValue<int>("Smtp:Port");
                    SmtpServer.Host = configuration.GetValue<string>("Smtp:Server");
                    SmtpServer.Send(mail);
                    isSend = true;
                }
            }
            catch (SmtpException ex)
            {
                string msg = ex.Message;
            }
            return isSend;
        }
    }
}
