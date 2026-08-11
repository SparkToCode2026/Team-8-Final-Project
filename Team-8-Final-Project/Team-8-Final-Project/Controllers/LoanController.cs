using Microsoft.AspNetCore.Mvc;
using Team_8_Final_Project.Models;

namespace Team_8_Final_Project.Controllers
{
    [ApiController]
    [Route("Loan")]
    public class LoanController : ControllerBase
    {


        private ProjectContext context;

        public LoanController(ProjectContext _context)
        {
            context = _context;
        }

        // Request URL => http://localhost:5240/Loan/AddLoan
        // Request method => POST
        // Request body => { "LoanStartDate": "2024-06-01T00:00:00", "LoanAmount": 100.0,
        // "LoanDueDate": "2024-06-15T00:00:00", "LoanReturnDate": null,
        // "loanStatus": 0, "BookCopyId": 1, "UserID": 1 }
        // Send request => Call function

        [HttpPost("AddLoan")]
        public void AddLoan(Loan l)
        {
            context.loans.Add(l);
            context.SaveChanges();
        }

        // Request URL => http://localhost:5240/Loan/RemoveLoan?id=2
        // Request method => DELETE
        // Request body => empty
        // Send request => Call function

        [HttpDelete("RemoveLoan")]
        public void RemoveLoan(int id)
        {
            Loan l = context.loans.FirstOrDefault(l => l.LoanId == id);

            if (l == null)
            {

            }
            else
            {
                context.loans.Remove(l);
                context.SaveChanges();
            }
        }

        [HttpGet("GetLoan")]
        public Loan GetLoan(int id)
        {
            Loan l = context.loans.FirstOrDefault(l => l.LoanId == id);
            return l;
        }

        [HttpGet("GetAllLoans")]
        public List<Loan> GetAllLoans()
        {
            List<Loan> loans = context.loans.ToList();
            return loans;
        }

        [HttpPatch("UpdateLoanAmount")]
        public void UpdateLoanAmount(int id, double newAmount)
        {
            Loan l = context.loans.FirstOrDefault(l => l.LoanId == id);

            l.LoanAmount = newAmount;
            context.SaveChanges();
        }

        [HttpPatch("UpdateLoanStatus")]
        public void UpdateLoanStatus(int id, LoanStatus newStatus)
        {
            Loan l = context.loans.FirstOrDefault(l => l.LoanId == id);
            l.loanStatus = newStatus;
            context.SaveChanges();
        }

        [HttpGet("GetLoansByUser")]
        public List<Loan> GetLoansByUser(int userId)
        {
            List<Loan> loans = context.loans.Where(l => l.UserID == userId).ToList();
            return loans;
        }
        
        [HttpGet("GetLoansByBookCopy")]
        public List<Loan> GetLoansByBookCopy(int bookCopyId)
        {
            List<Loan> loans = context.loans.Where(l => l.BookCopyId == bookCopyId).ToList();
            return loans;
        }


    }
}