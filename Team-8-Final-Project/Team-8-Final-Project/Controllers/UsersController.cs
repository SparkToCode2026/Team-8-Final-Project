using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Team_8_Final_Project.Models;

namespace Team_8_Final_Project.Controllers
{
    [ApiController]
    [Route("User")]
    public class UsersController : ControllerBase
    {
        private ProjectContext context;

        public UsersController(ProjectContext _context)
        {
            context = _context;
        }

        public class UpdateUserDto
        {
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string UserEmail { get; set; }
            public string UserPhoneNo { get; set; }
            public DateTime DOB { get; set; }
        }

        // Update a user's profile details (name, email, phone, date of birth)
        // Does not touch Role or PasswordHash - those are handled separately
        [HttpPut("UpdateUser")]
        [Authorize(Roles = "Admin, Librarian")]
        public IActionResult UpdateUser(int id, UpdateUserDto dto)
        {
            User existingUser = context.Users.FirstOrDefault(u => u.UserId == id);

            if (existingUser == null)
            {
                return NotFound("User not found.");
            }

            existingUser.FirstName = dto.FirstName;
            existingUser.LastName = dto.LastName;
            existingUser.UserEmail = dto.UserEmail;
            existingUser.UserPhoneNo = dto.UserPhoneNo;
            existingUser.DOB = dto.DOB;

            context.SaveChanges();

            return Ok(existingUser);
        }

        // Change a user's role - Admin only, since this controls system access level
        [HttpPatch("ChangeUserRole")]
        [Authorize(Roles = "Admin")]
        public IActionResult ChangeUserRole(int id, UserRole newRole)
        {
            User existingUser = context.Users.FirstOrDefault(u => u.UserId == id);

            if (existingUser == null)
            {
                return NotFound("User not found.");
            }

            existingUser.Role = newRole;
            context.SaveChanges();

            return Ok(existingUser);
        }

        // Remove a user account
        [HttpDelete("RemoveUser")]
        [Authorize(Roles = "Admin")]
        public IActionResult RemoveUser(int id)
        {
            User existingUser = context.Users.FirstOrDefault(u => u.UserId == id);

            if (existingUser == null)
            {
                return NotFound("User not found.");
            }

            context.Users.Remove(existingUser);
            context.SaveChanges();

            return Ok("User account removed.");
        }

        // Get all users, including their loans and reservations
        [HttpGet("FetchAllUsers")]
        [Authorize(Roles = "Admin, Librarian")]
        public IActionResult FetchAllUsers()
        {
            List<User> users = context.Users.Include(u => u.Loans)
                                            .Include(u => u.Reservations)
                                            .ToList();

            return Ok(users);
        }

        // Get a single user by Id
        [HttpGet("GetUserById")]
        [Authorize(Roles = "Admin, Librarian")]
        public IActionResult GetUserById(int id)
        {
            User user = context.Users.Include(u => u.Loans)
                                     .Include(u => u.Reservations)
                                     .FirstOrDefault(u => u.UserId == id);

            if (user == null)
            {
                return NotFound("User not found.");
            }

            return Ok(user);
        }

        // Filter users by Role
        [HttpGet("FilterUsersByRole")]
        [Authorize(Roles = "Admin, Librarian")]
        public IActionResult GetUsersByRole(UserRole role)
        {
            List<User> users = context.Users.Where(u => u.Role == role).ToList();

            return Ok(users);
        }

        // Get total user count, plus a sorted summary list
        [HttpGet("GetUsersSummary")]
        [Authorize(Roles = "Admin, Librarian")]
        public IActionResult GetUsersSummary()
        {
            int totalCount = context.Users.Count();
            var sortedList = context.Users.OrderBy(u => u.LastName)
                                          .Select(u => new
                                          {
                                              u.UserId,
                                              FullName = u.FirstName + " " + u.LastName,
                                              u.UserEmail,
                                              u.Role
                                          })
                                          .ToList();

            return Ok(new
            {
                Total = totalCount,
                Data = sortedList
            });
        }
    }
}