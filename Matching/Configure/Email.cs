using System.Net.Mail;
using System.Net;

namespace Matching.Configure
{
    public class Email
    {
        public async void SendEmail(string toEmail,string subject, string body)
        {
            try
            {

                using (var smtpClient = new SmtpClient("smtp.gmail.com"))
                {
                    smtpClient.Port = 587;
                    smtpClient.Credentials = new NetworkCredential("lovelockdowndating@gmail.com", "peau opau qnvy xcbj");
                    smtpClient.EnableSsl = true;
                    smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
                    smtpClient.UseDefaultCredentials = false;
                    var message = new MailMessage("lovelockdowndating@gmail.com", toEmail, subject, body);
                    message.IsBodyHtml = true;
                    await smtpClient.SendMailAsync(message);
                }
            }
            catch
            {

            }
        }
    }
}
