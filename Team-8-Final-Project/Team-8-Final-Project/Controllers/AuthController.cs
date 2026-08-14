using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Team_8_Final_Project.EmailServices;
using Team_8_Final_Project.Models;
namespace Team_8_Final_Project.Controllers
{
    [ApiController]
    [Route("Auth")]
    public class AuthController : ControllerBase
    {
        private ProjectContext context;
        private IConfiguration configuration;
        private EmailService emailService;

        public AuthController(ProjectContext _context, IConfiguration _configuration, EmailService _emailService)
        {
            context = _context;
            configuration = _configuration;
            emailService = _emailService;
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
            public string UserPhoneNo { get; set; }

            [Required]
            public DateTime DOB { get; set; }

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
                UserPhoneNo = registerDto.UserPhoneNo,
                DOB = registerDto.DOB,
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

        public class LoginDto
        {
            [Required]
            [EmailAddress]
            public string UserEmail { get; set; }

            [Required]
            public string Password { get; set; }
        }

        // Login
        [HttpPost("Login")]
        public IActionResult Login(LoginDto loginDto)
        {
            // Find the user by email
            User user = context.Users.FirstOrDefault(u => u.UserEmail == loginDto.UserEmail);

            if (user == null)
            {
                return Unauthorized("Invalid email or password.");
            }

            // Verify the password
            PasswordHasher<User> hasher = new PasswordHasher<User>();

            PasswordVerificationResult result = hasher.VerifyHashedPassword(
                user,
                user.PasswordHash,
                loginDto.Password
            );

            if (result == PasswordVerificationResult.Failed)
            {
                return Unauthorized("Invalid email or password.");
            }

            // Create claims
            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                new Claim(ClaimTypes.Email, user.UserEmail),
                new Claim(ClaimTypes.Role, user.Role.ToString())
            };

            // Get JWT key from appsettings.json
            SymmetricSecurityKey key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(configuration["Jwt:Key"]!)
            );

            // Create the signing credentials
            SigningCredentials credentials = new SigningCredentials(
                key,
                SecurityAlgorithms.HmacSha256
            );

            // Generate the JWT
            JwtSecurityToken token = new JwtSecurityToken(
                issuer: configuration["Jwt:Issuer"],
                audience: configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddHours(2),
                signingCredentials: credentials
            );

            // Convert the token to string
            string tokenString = new JwtSecurityTokenHandler().WriteToken(token);

            return Ok(new
            {
                Message = "Login successful.",
                Token = tokenString
            });
        }

        public class ForgotPasswordDto
        {
            [Required]
            [EmailAddress]
            public string UserEmail { get; set; }
        }

        // Forgot Password
        [HttpPost("ForgotPassword")]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordDto forgotPasswordDto)
        {
            // Find the user by email
            User user = context.Users.FirstOrDefault(u => u.UserEmail == forgotPasswordDto.UserEmail);

            if (user == null)
            {
                return NotFound("No account was found with this email.");
            }

            // Generate a secure random reset token
            byte[] tokenBytes = new byte[32];

            using (RandomNumberGenerator random = RandomNumberGenerator.Create())
            {
                random.GetBytes(tokenBytes);
            }

            string resetToken = Convert.ToBase64String(tokenBytes);

            // Save the token and expiration time
            user.PasswordResetToken = resetToken;
            user.PasswordResetTokenExpiry = DateTime.Now.AddMinutes(30);

            context.SaveChanges();

            string resetLink = configuration["EmailSettings:ResetPasswordUrl"]
                           + "?email=" + Uri.EscapeDataString(user.UserEmail)
                           + "&token="  + Uri.EscapeDataString(resetToken);

            string body =
             "Hello " + user.FirstName + ",\n\n" +
             "We received a request to reset your password for the Library Management System.\n\n" +
             "Please use the following link to reset your password:\n" +
              resetLink + "\n\n" +
             "This link will expire in 30 minutes.\n\n" +
             "If you did not request a password reset, you can ignore this email.\n\n" +
             "Library Management System";

            await emailService.SendEmailAsync( user.UserEmail,
                                               "Password Reset - Library Management System",
                                               body );

        }

        public class ResetPasswordDto
        {
            [Required]
            [EmailAddress]
            public string UserEmail { get; set; }

            [Required]
            public string ResetToken { get; set; }

            [Required]
            [MinLength(8, ErrorMessage = "Password must be at least 8 characters long.")]
            public string NewPassword { get; set; }
        }

        // Reset Password
        [HttpPost("ResetPassword")]
        public IActionResult ResetPassword(ResetPasswordDto resetPasswordDto)
        {
            // Find the user
            User user = context.Users.FirstOrDefault(u => u.UserEmail == resetPasswordDto.UserEmail);

            if (user == null)
            {
                return NotFound("User not found.");
            }

            // Check reset request exists
            if (string.IsNullOrEmpty(user.PasswordResetToken))
            {
                return BadRequest("No password reset request was found.");
            }

            // Check if token expired
            if (user.PasswordResetTokenExpiry == null || user.PasswordResetTokenExpiry < DateTime.Now)
            {
                // Invalidate expired token
                user.PasswordResetToken = null;
                user.PasswordResetTokenExpiry = null;

                context.SaveChanges();

                return BadRequest("The password reset link has expired. Please request a new one.");
            }

            // Check that the token from the link matches the stored token
            if (user.PasswordResetToken != resetPasswordDto.ResetToken)
            {
                return BadRequest("Invalid password reset link.");
            }

            // Hash the new password
            PasswordHasher<User> hasher = new PasswordHasher<User>();

            user.PasswordHash = hasher.HashPassword(
                user,
                resetPasswordDto.NewPassword
            );

            // Reset/Invalidate token after successful password reset
            user.PasswordResetToken = null;
            user.PasswordResetTokenExpiry = null;

            context.SaveChanges();

            return Ok("Password has been reset successfully.");
        }

    }


}
