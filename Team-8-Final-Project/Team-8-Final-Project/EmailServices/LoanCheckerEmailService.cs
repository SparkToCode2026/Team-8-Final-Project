using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Team_8_Final_Project.Models;

namespace Team_8_Final_Project.EmailServices
{
    public class LoanCheckerEmailService : BackgroundService
    {
        private readonly IServiceScopeFactory scopeFactory;
        private readonly EmailService emailService;

        public LoanCheckerEmailService( IServiceScopeFactory _scopeFactory, EmailService _emailService)
        {
            scopeFactory = _scopeFactory;
            emailService = _emailService;
        }
    }
}
