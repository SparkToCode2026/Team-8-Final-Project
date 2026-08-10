using Microsoft.AspNetCore.Mvc;
using Team_8_Final_Project.Models;
namespace Team_8_Final_Project.Controllers
{
    [ApiController]
    [Route("Book")]
    public class BookController : ControllerBase
    {
        private ProjectContext context;
        public BookController(ProjectContext _context)
        {
            context = _context;
        }
    }
}
