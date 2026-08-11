using Microsoft.AspNetCore.Mvc;

namespace Team_8_Final_Project.Controllers
{
    public class LoanController
    {
        public class LoanController : ControllerBase
        {
            private ProjectContext context;

            public LoanController(ProjectContext _context) 
            {
                context = _context;
            }
        }
    }
}
