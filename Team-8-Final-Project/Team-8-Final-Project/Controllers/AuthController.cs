using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Team_8_Final_Project.Models;
namespace Team_8_Final_Project.Controllers
{
    [ApiController]
    [Route("AuthController")]
    public class AuthController : ControllerBase
    {
        private ProjectContext context;

        public AuthController(ProjectContext _context)
        {
            context = _context;
        }

        // A test 
        [HttpGet("TestPasswordHasher")]
        public IActionResult TestPasswordHasher()
        {
            PasswordHasher<User> hasher = new PasswordHasher<User>();

            return Ok("PasswordHasher is working!");
        }
    }


}
