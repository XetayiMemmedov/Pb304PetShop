using MimeKit;
using Microsoft.Extensions.Configuration;
using MailKit.Net.Smtp;
using System;
using Pb304PetShop.Models; // Assuming your Mail and MailSettings classes are here

namespace Pb304PetShop.MailKitImplementations
{
    public class MailKitMailService : IMailService
    {
        private readonly MailSettings _mailSettings;

        public MailKitMailService(IConfiguration configuration)
        {
            _mailSettings = configuration.GetSection("MailSettings").Get<MailSettings>()
                            ?? throw new ArgumentNullException("MailSettings section is missing in configuration.");
        }

        public void SendMail(Mail mail)
        {
            if (mail == null) throw new ArgumentNullException(nameof(mail), "Email not defined.");

            var email = new MimeMessage();
            email.From.Add(new MailboxAddress(_mailSettings.SenderFullName,_mailSettings.SenderEmail));
            email.To.Add(new MailboxAddress(mail.FullName, mail.Email));
            email.Subject = mail.Subject;

            var bodyBuilder = new BodyBuilder
            {
                TextBody = mail.TextBody,
                HtmlBody = mail.HtmlBody
            };

            if (mail.Attachments != null)
            {
                foreach (var attachment in mail.Attachments)
                {
                    if (attachment is MimeEntity entity)
                        bodyBuilder.Attachments.Add(entity);
                }
            }

            email.Body = bodyBuilder.ToMessageBody();

            using var smtp = new SmtpClient();
            smtp.Connect(_mailSettings.Server, _mailSettings.Port, false);
            smtp.Authenticate(_mailSettings.UserName, _mailSettings.Password);
            smtp.Send(email);
            smtp.Disconnect(true);
        }
    }
}
