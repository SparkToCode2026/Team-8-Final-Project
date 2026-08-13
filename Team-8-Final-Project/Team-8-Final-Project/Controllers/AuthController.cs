using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
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

        public class RegisterDto
        {
            [Required]
            public string FirstName { get; set; }

            [Required]
            public string LastName { get; set; }

            [Required]
            public string UserEmail { get; set; }

            [Required]
            public string Password { get; set; }
        }

    }


}
