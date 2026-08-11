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

        [HttpPost("AddLoan")]
        public void AddLoan(Loan l)
        {
            context.loans.Add(l);
            context.SaveChanges();
        }

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
    }
}
