using System;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Demo.RentalRepairs.Core.Interfaces;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace Demo.RentalRepairs.Infrastructure.Email
{
    public class SendGridEmailService : IEmailService
    {
        private readonly string _senderEmail = "demo-rental-repairs-no-reply@protonmail.com";
        private readonly string _recipientEmail = "akhmelevtsov@gmail.com";

        public async Task SendEmailAsync(EmailInfo email)
        {
            if (email == null)
                throw new ArgumentNullException(nameof(email));

            //using (var message = new MailMessage())
            //{
            //    message.To.Add(new MailAddress(email.RecipientEmail , "To Name"));
            //    message.From = new MailAddress(email.SenderEmail , "From Name");
            //    //message.CC.Add(new MailAddress("cc@email.com", "CC Name"));
            //    //message.Bcc.Add(new MailAddress("bcc@email.com", "BCC Name"));
            //    //message.Subject = $"{email.Subject}- [{email.SenderEmail}->{email.RecipientEmail}]";
            //    message.Subject = $"{email.Subject}";
            //    message.Body = email.Body ;
            //    message.IsBodyHtml = true;
            //    message.BodyEncoding = System.Text.Encoding.UTF8;

            //    //using (var client = new SmtpClient("smtp.gmail.com"))
            //    using (var client = new SmtpClient("localhost"))
            //    {
            //        //client.Port = 587;
            //        client.Port = 2500;
            //        //client.Credentials = new NetworkCredential("akhmelevtsov@gmail.com", "");
            //        //client.EnableSsl = true;
            //        client.Send(message);
            //    }
            //}
            var apiKey = Environment.GetEnvironmentVariable("SENDGRID_API_KEY");
            var client = new SendGridClient(apiKey);
            var from = new EmailAddress(_senderEmail, "Example User");
            var subject = "Sending with Twilio SendGrid is Fun";
            var to = new EmailAddress(_recipientEmail, "Example User");
            var plainTextContent = "and easy to do anywhere, even with C#";
            var htmlContent = "<strong>and easy to do anywhere, even with C#</strong>";
            var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
            var response = await client.SendEmailAsync(msg).ConfigureAwait(false);
        }
    }
}
