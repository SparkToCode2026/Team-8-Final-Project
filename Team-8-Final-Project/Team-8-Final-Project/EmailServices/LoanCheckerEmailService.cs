using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Team_8_Final_Project.Models;

namespace Team_8_Final_Project.EmailServices
{
    public class LoanCheckerEmailService : BackgroundService
    {
        private readonly IServiceScopeFactory scopeFactory;
        private readonly EmailService emailService;
        private readonly IConfiguration configuration;

        public LoanCheckerEmailService( IServiceScopeFactory _scopeFactory, EmailService _emailService, IConfiguration _configuration)
        {
            scopeFactory = _scopeFactory;
            emailService = _emailService;
            configuration = _configuration;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            int intervalMinutes = int.Parse( configuration["LoanCheckerSettings:CheckIntervalMinutes"]! );
            while (!stoppingToken.IsCancellationRequested)
            {
                using (IServiceScope scope = scopeFactory.CreateScope())
                {
                    ProjectContext context = scope.ServiceProvider.GetRequiredService<ProjectContext>();
                }

                await Task.Delay( TimeSpan.FromMinutes(intervalMinutes), stoppingToken );
            }
        }
    }
}
