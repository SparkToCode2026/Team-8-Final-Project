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
    }
}
