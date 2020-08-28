using System;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Demo.RentalRepairs.Core.Interfaces;

namespace Demo.RentalRepairs.Infrastructure.Email
{
    public class MailSlurperEmailService : IEmailService
    {
        

        public async Task SendEmailAsync(EmailInfo email)
        {
            await Task.CompletedTask;
            if (email == null)
                throw new ArgumentNullException(nameof(email));
            
            using (var message = new MailMessage())
            {
                message.To.Add(new MailAddress(email.RecipientEmail , "To Name"));
                message.From = new MailAddress(email.SenderEmail , "From Name");
                //message.CC.Add(new MailAddress("cc@email.com", "CC Name"));
                //message.Bcc.Add(new MailAddress("bcc@email.com", "BCC Name"));
                //message.Subject = $"{email.Subject}- [{email.SenderEmail}->{email.RecipientEmail}]";
                message.Subject = $"{email.Subject}";
                message.Body = email.Body ;
                message.IsBodyHtml = true;
                message.BodyEncoding = System.Text.Encoding.UTF8;

                //using (var client = new SmtpClient("smtp.gmail.com"))
                using (var client = new SmtpClient("localhost"))
                {
                    //client.Port = 587;
                    client.Port = 2500;
                    //client.Credentials = new NetworkCredential("", "");
                    //client.EnableSsl = true;
                    client.Send(message);
                }
            }
        }
    }
}
