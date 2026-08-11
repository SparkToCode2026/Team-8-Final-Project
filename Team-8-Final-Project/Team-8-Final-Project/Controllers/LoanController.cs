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
        public IActionResult AddLoan(Loan l)
        {
            context.Loans.Add(l);
            context.SaveChanges();
            return Ok(l.LoanId);
        }

        // Request URL => http://localhost:5240/Loan/RemoveLoan?id=2
        // Request method => DELETE
        // Request body => empty
        // Send request => Call function

        [HttpDelete("RemoveLoan")]
        public IActionResult RemoveLoan(int id)
        {
            Loan l = context.Loans.FirstOrDefault(l => l.LoanId == id);

            if (l == null)
            {
                return NotFound("Loan not found");
            }
            else
            {
                context.Loans.Remove(l);
                context.SaveChanges();
                return Ok("Loan removed successfully");
            }
        }

        [HttpGet("GetLoan")]
        public IActionResult GetLoan(int id)
        {
            Loan l = context.Loans.FirstOrDefault(l => l.LoanId == id);
            return Ok(l);
        }

        [HttpGet("GetAllLoans")]
        public IActionResult GetAllLoans()
        {
            List<Loan> loans = context.Loans.ToList();
            return Ok(loans);
        }

        

        [HttpPatch("UpdateLoanStatus")]
        public IActionResult UpdateLoanStatus(int id, LoanStatus newStatus)
        {
            Loan l = context.Loans.FirstOrDefault(l => l.LoanId == id);
            l.loanStatus = newStatus;
            context.SaveChanges();
            return Ok();
        }

        [HttpGet("GetLoansByUser")]
        public IActionResult GetLoansByUser(int userId)
        {
            List<Loan> loans = context.Loans.Where(l => l.UserID == userId).ToList();
            return loans;
        }

        [HttpGet("GetLoansByBookCopy")]
        public IActionResult GetLoansByBookCopy(int bookCopyId)
        {
            List<Loan> loans = context.Loans.Where(l => l.BookCopyId == bookCopyId).ToList();
            return loans;
        }

        [HttpPut("UpdateLoan")]
        public IActionResult UpdateLoan(int id, Loan newLoan)
        {
            Loan l = context.Loans.FirstOrDefault(l => l.LoanId == id);
            if (l != null)
            {
                l.LoanStartDate = newLoan.LoanStartDate;
                l.LoanDueDate = newLoan.LoanDueDate;
                l.LoanReturnDate = newLoan.LoanReturnDate;
                l.loanStatus = newLoan.loanStatus;
                l.BookCopyId = newLoan.BookCopyId;
                l.UserID = newLoan.UserID;

                context.SaveChanges();
            }


        }
    }
}