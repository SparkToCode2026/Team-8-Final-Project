using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
namespace Team_8_Final_Project.EmailServices
{
    public class EmailService
    {
        private readonly IConfiguration configuration;

        public EmailService(IConfiguration _configuration)
        {
            configuration = _configuration;
        }

        public async Task SendEmailAsync( string recipientEmail, string subject, string body)
        {
            MimeMessage email = new MimeMessage();

            email.From.Add(new MailboxAddress(configuration["EmailSettings:SenderName"],
                                              configuration["EmailSettings:SenderEmail"] ));

            email.To.Add(MailboxAddress.Parse(recipientEmail));
            email.Subject = subject;
            email.Body = new TextPart("html") 
            {
                Text = body 
            };

            using (SmtpClient smtp = new SmtpClient())
            {
                await smtp.ConnectAsync( configuration["EmailSettings:SmtpServer"],
                                         int.Parse(configuration["EmailSettings:Port"]!),
                                         SecureSocketOptions.StartTls );

                await smtp.AuthenticateAsync( configuration["EmailSettings:SenderEmail"],
                                              configuration["EmailSettings:Password"] );

                await smtp.SendAsync(email);
            }
        }
    }
}
