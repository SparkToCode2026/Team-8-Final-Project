using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
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
            decimal fineAmountPerDay = decimal.Parse( configuration["LoanCheckerSettings:FineAmountPerDay"]! );

            while (!stoppingToken.IsCancellationRequested)
            {
                using (IServiceScope scope = scopeFactory.CreateScope())
                {
                    ProjectContext context = scope.ServiceProvider.GetRequiredService<ProjectContext>();

                    List<Loan> activeLoans = context.Loans.Include(l => l.User)
                                                          .Include(l => l.BookCopy)
                                                             .ThenInclude(bc => bc.Book)
                                                          .Where(l => l.loanStatus == LoanStatus.Active)
                                                          .ToList();

                    foreach (Loan loan in activeLoans)
                    {
                        if (loan.LoanDueDate < DateTime.Now)
                        {
                            loan.loanStatus = LoanStatus.Overdue;

                            Fine existingFine = context.Fines.FirstOrDefault(f => f.LoanId == loan.LoanId);

                            int overdueDays = (DateTime.Now.Date - loan.LoanDueDate.Date).Days;

                            decimal totalFine = overdueDays * fineAmountPerDay;

                            if (existingFine == null)
                            {
                                Fine fine = new Fine
                                {
                                    LoanId = loan.LoanId,
                                    FineAmount = totalFine,
                                    Status = FinePaymentStatus.Unpaid,
                                    FineIssueDate = DateTime.Now
                                };

                                context.Fines.Add(fine);
                            }
                            else
                            {
                                existingFine.FineAmount = totalFine;
                            }
                        }
                        else if (loan.LoanDueDate.Date == DateTime.Now.Date.AddDays(1))
                        {
                            string body =
                              "Hello " + loan.User.FirstName + ",\n\n" +
                              "This is a reminder that your borrowed book is due tomorrow.\n\n" +
                              "Book: " + loan.BookCopy.Book.BookTitle + "\n" +
                              "Due date: " + loan.LoanDueDate.ToShortDateString() + "\n\n" +
                              "Please return the book on time to avoid an overdue fine.\n\n" +
                              "Library Management System";

                            await emailService.SendEmailAsync( loan.User.UserEmail,
                             "Library Loan Reminder",
                               body );
                        }

                    }

                }

                await Task.Delay( TimeSpan.FromMinutes(intervalMinutes), stoppingToken );
            }
        }
    }
}
