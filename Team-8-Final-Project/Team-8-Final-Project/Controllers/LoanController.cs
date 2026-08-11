using Microsoft.AspNetCore.Mvc;
using Team_8_Final_Project.Models;

namespace Team_8_Final_Project.Controllers
{
    [ApiController]
    [Route("Loan")]
    public class LoanController
    {
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
        }
    }
}
