using Microsoft.AspNetCore.Mvc;
using Team_8_Final_Project.Models;
namespace Team_8_Final_Project.Controllers
{
    [ApiController]
    [Route("BookCopy")]
    public class BookCopyController : ControllerBase
    {
        private ProjectContext context;
        public BookCopyController(ProjectContext _context)
        {
            context = _context;
        }
    }
}
