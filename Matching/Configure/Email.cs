using System.Net.Mail;
using System.Net;

namespace Matching.Configure
{
    public class Email
    {
        public void SendResetEmail(string toEmail,string subject, string body)
        {
            using (var smtpClient = new SmtpClient("smtp-mail.outlook.com"))
            {
                smtpClient.Port = 587;
                smtpClient.Credentials = new NetworkCredential("lovelockdownwebsite@outlook.com", "ammar12345");
                smtpClient.EnableSsl = true;
                smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
                smtpClient.UseDefaultCredentials = false;
                var message = new MailMessage("lovelockdownwebsite@outlook.com", toEmail, subject, body);
                message.IsBodyHtml = true;
                smtpClient.Send(message);
            }
        }
    }
}
