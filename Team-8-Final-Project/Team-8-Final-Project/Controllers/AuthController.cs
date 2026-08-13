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
            [EmailAddress]
            public string UserEmail { get; set; }

            [Required]
            [MinLength(8, ErrorMessage = "Password must be at least 8 characters long.")]
            public string Password { get; set; }
        }

        // Register a new user
        [HttpPost("Register")]
        public IActionResult Register(RegisterDto registerDto)
        {
            // Check if email already exists
            User existingUser = context.Users.FirstOrDefault(u => u.UserEmail == registerDto.UserEmail);

            if (existingUser != null)
            {
                return BadRequest("Email is already registered.");
            }

            // Create the User
            User user = new User
            {
                FirstName = registerDto.FirstName,
                LastName = registerDto.LastName,
                UserEmail = registerDto.UserEmail,
                Role = UserRole.Member
            };

            // Hash the password
            PasswordHasher<User> hasher = new PasswordHasher<User>();

            user.PasswordHash = hasher.HashPassword(
                user,
                registerDto.Password
            );

            context.Users.Add(user);
            context.SaveChanges();

            return Ok("User registered successfully.");
        }

    }


}
