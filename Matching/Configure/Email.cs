using System.Net.Mail;
using System.Net;

namespace Matching.Configure
{
    public class Email
    {
        public async void SendEmail(string toEmail,string subject, string body)
        {
            using (var smtpClient = new SmtpClient("smtp-mail.outlook.com"))
            {
                smtpClient.Port = 587;
                smtpClient.Credentials = new NetworkCredential("lovelockdownwebsite@hotmail.com", "jpeaojjzcperyhpy");
                smtpClient.EnableSsl = true;
                smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
                smtpClient.UseDefaultCredentials = false;
                var message = new MailMessage("lovelockdownwebsite@hotmail.com", toEmail, subject, body);
                message.IsBodyHtml = true;
                await smtpClient.SendMailAsync(message);
            }
        }
    }
}
