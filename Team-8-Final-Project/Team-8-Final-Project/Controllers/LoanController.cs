using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
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

        public class CreateLoanDto
        {
            [Required]
            public DateTime LoanDueDate { get; set; }

            [Required]
            public int BookCopyId { get; set; }

            [Required]
            public int UserID { get; set; }
        }

        public class UpdateLoanDto
        {
            [Required]
            public DateTime LoanStartDate { get; set; }

            [Required]
            public DateTime LoanDueDate { get; set; }

            [Required]
            public int BookCopyId { get; set; }

            [Required]
            public int UserID { get; set; }
        }

        // Request URL => http://localhost:5240/Loan/AddLoan
        // Request method => POST
        // Request body => { "LoanStartDate": "2024-06-01T00:00:00",
        // "LoanDueDate": "2024-06-15T00:00:00", "LoanReturnDate": null,
        // "loanStatus": 0, "BookCopyId": 1, "UserID": 1 }
        // Send request => Call function

        [HttpPost("AddLoan")]
        [Authorize(Roles = "Librarian,Admin")]
        public IActionResult AddLoan(CreateLoanDto dto)
        {
            var loan = new Loan
            {
                LoanStartDate = DateTime.Now,
                LoanDueDate = dto.LoanDueDate,
                BookCopyId = dto.BookCopyId,
                UserID = dto.UserID,
                loanStatus = LoanStatus.Active
            };
            context.Loans.Add(loan);
            context.SaveChanges();
            return Ok(loan);
        }

        // Request URL => http://localhost:5240/Loan/RemoveLoan?id=2
        // Request method => DELETE
        // Request body => empty
        // Send request => Call function

        [HttpDelete("RemoveLoan")]
        [Authorize(Roles = "Admin")]
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
        [Authorize(Roles = "Librarian,Admin")]
        public IActionResult GetLoan(int id)
        {
            Loan l = context.Loans
        .Include(l => l.BookCopy).ThenInclude(bc => bc.Book)
        .Include(l => l.User)
        .FirstOrDefault(l => l.LoanId == id);
            if (l == null)
            {
                return NotFound("Loan Id not found");
            }
            else
            {
                return Ok(l);
            }
            
        }

        [HttpGet("GetAllLoans")]
        [Authorize(Roles = "Librarian,Admin")]
        public IActionResult GetAllLoans()
        {
            List<Loan> loans = context.Loans.Include(l => l.BookCopy)
                                            .Include(l => l.User)
                                            .ToList();
            return Ok(loans);
        }

        

        [HttpPatch("UpdateLoanStatus")]
        [Authorize(Roles = "Librarian,Admin")]
        public IActionResult UpdateLoanStatus(int id, LoanStatus newStatus)
        {
            Loan l = context.Loans.FirstOrDefault(l => l.LoanId == id);
            if (l == null)
            {
                return NotFound();
            }
            else
            {
                l.loanStatus = newStatus;
                context.SaveChanges();
                return Ok();
            }
        }

        [HttpGet("GetLoansByUser")]
        [Authorize]
        public IActionResult GetLoansByUser(int userId)
        {
            List<Loan> loans = context.Loans.Where(l => l.UserID == userId).ToList();
            return Ok(loans);
        }

        [HttpGet("GetLoansByBookCopy")]
        [Authorize(Roles = "Librarian,Admin")]
        public IActionResult GetLoansByBookCopy(int bookCopyId)
        {
            List<Loan> loans = context.Loans.Where(l => l.BookCopyId == bookCopyId).ToList();
            return Ok(loans);
        }

        [HttpPut("UpdateLoan")]
        [Authorize(Roles = "Librarian,Admin")]
        public IActionResult UpdateLoan(int id, UpdateLoanDto dto)
        {
            Loan l = context.Loans.FirstOrDefault(l => l.LoanId == id);
            if (l == null) return NotFound("Loan not found");

            l.LoanStartDate = dto.LoanStartDate;
            l.LoanDueDate = dto.LoanDueDate;
            l.BookCopyId = dto.BookCopyId;
            l.UserID = dto.UserID;

            context.SaveChanges();
            return Ok(l);
        }

        [HttpGet("GetLoansSortedByDueDate")]
        [Authorize(Roles = "Librarian,Admin")]
        public IActionResult GetLoansSortedByDueDate()
        {
            List<Loan> loans = context.Loans.OrderBy(l => l.LoanDueDate).ToList();
            return Ok(loans);
        }
    }
}