using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Team_8_Final_Project.Models;

namespace Team_8_Final_Project.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly ProjectContext _db;

        public UsersController(ProjectContext db)
        {
            _db = db;
        }

        // 1. POST: Add new User
        [HttpPost]
        public async Task<ActionResult<User>> AddUser(User newUser)
        {
            _db.Users.Add(newUser);
            await _db.SaveChangesAsync();
            return CreatedAtAction(nameof(GetUserById), new { id = newUser.UserID }, newUser);
        }

        // 2. PUT: Update full User
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, User updatedUser)
        {
            if (id != updatedUser.UserID) return BadRequest("ID Mismatch");

            _db.Entry(updatedUser).State = EntityState.Modified;

            try
            {
                await _db.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_db.Users.Any(u => u.UserID == id)) return NotFound();
                throw;
            }

            return NoContent();
        }

        // 3. PATCH: Update User Role
        [HttpPatch("{id}/role")]
        public async Task<IActionResult> ChangeUserRole(int id, [FromBody] UserRole newRole)
        {
            var user = await _db.Users.FindAsync(id);
            if (user == null) return NotFound();

            user.Role = newRole;
            await _db.SaveChangesAsync();

            return Ok(user);
        }

        // 4. DELETE: Remove User
        [HttpDelete("{id}")]
        public async Task<IActionResult> RemoveUser(int id)
        {
            var user = await _db.Users.FindAsync(id);
            if (user == null) return NotFound();

            _db.Users.Remove(user);
            await _db.SaveChangesAsync();

            return Ok(new { Message = "User account removed." });
        }

        // 5. GET List: Fetch all users with related Loans and Reservations
        [HttpGet]
        public async Task<ActionResult<IEnumerable<User>>> FetchAllUsers()
        {
            return await _db.Users
                .Include(u => u.Loans)
                .Include(u => u.Reservations)
                .ToListAsync();
        }

        // 6. GET Find: Get user by ID
        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetUserById(int id)
        {
            var user = await _db.Users
                .Include(u => u.Loans)
                .Include(u => u.Reservations)
                .FirstOrDefaultAsync(u => u.UserID == id);

            if (user == null) return NotFound();

            return user;
        }

        // 7. GET Filter: Filter users by Role (Enum)
        [HttpGet("filter-by-role/{role}")]
        public async Task<ActionResult<IEnumerable<User>>> GetUsersByRole(UserRole role)
        {
            return await _db.Users
                .Where(u => u.Role == role)
                .ToListAsync();
        }

        // 8. GET Aggregate/Sort: Get total count and sorted users
        [HttpGet("summary")]
        public async Task<IActionResult> GetUsersSummary()
        {
            var totalCount = await _db.Users.CountAsync();
            var sortedList = await _db.Users
                .OrderBy(u => u.LastName)
                .Select(u => new
                {
                    u.UserID,
                    FullName = u.FirstName + " " + u.LastName,
                    u.UserEmail,
                    u.Role
                })
                .ToListAsync();

            return Ok(new
            {
                Total = totalCount,
                Data = sortedList
            });
        }
    }
}