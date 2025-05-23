using System.Net.Mail;
using MimeKit;

namespace Pb304PetShop
{
    public class Mail
    {
        public string? Subject {  get; set; }
        public string? TextBody { get; set; }
        public string? HtmlBody { get; set; }
        public string? FullName { get; set; }
        public required string Email { get; set; }
        public List<MimeEntity>? Attachments { get; set; }

        public Mail()
        {

        }

        public Mail(string? subject, string? textBody, string? htmlBody, string? fullName, string email, List<MimeEntity>? attachments)
        {
            Subject = subject;
            TextBody = textBody;
            HtmlBody = htmlBody;
            FullName = fullName;
            Email = email;
            Attachments = attachments;
        }
    }
}
