using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Team_8_Final_Project.Models;
using Microsoft.AspNetCore.Authorization; 
using System.Security.Claims; 

namespace Team_8_Final_Project.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FineController : ControllerBase
    {
        private readonly ProjectContext _context;

        public FineController(ProjectContext context)
        {
            _context = context;
        }
        // Case 1 - Create Fine
        [HttpPost]
        [Authorize(Roles = "Librarian,Admin")]
        public async Task<IActionResult> CreateFine(Fine fine)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            fine.Status = FinePaymentStatus.Unpaid;

            _context.Fines.Add(fine);
            await _context.SaveChangesAsync();

            return Ok(fine);
        }
        // Case 2 - Update Fine Information
        [HttpPut("{id}")]
        [Authorize(Roles = "Librarian,Admin")]
        public async Task<IActionResult> UpdateFine(int id, Fine updatedFine)
        {
            var fine = await _context.Fines.FindAsync(id);

            if (fine == null)
            {
                return NotFound();
            }

            fine.FineAmount = updatedFine.FineAmount;
            fine.FineIssueDate = updatedFine.FineIssueDate;

            await _context.SaveChangesAsync();

            return Ok(fine);
        }
        // Case 3 - Update Fine Status
        [HttpPatch("{id}/status")]
        [Authorize(Roles = "Librarian,Admin")]
        public async Task<IActionResult> UpdateFineStatus(int id, FinePaymentStatus status)
        {
            var fine = await _context.Fines.FindAsync(id);

            if (fine == null)
            {
                return NotFound();
            }

            fine.Status = status;

            await _context.SaveChangesAsync();

            return Ok(fine);
        }
        // Case 4 - Delete Fine
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteFine(int id)
        {
            var fine = await _context.Fines.FindAsync(id);

            if (fine == null)
            {
                return NotFound();
            }

            _context.Fines.Remove(fine);
            await _context.SaveChangesAsync();

            return Ok("Fine deleted successfully.");
        }
        // Case 5 - Get All Fines with Loan Details
        [HttpGet]
        [Authorize(Roles = "Librarian,Admin")]
        public async Task<IActionResult> GetAllFines()
        {
            var fines = await _context.Fines
                .Include(f => f.loan)
               .ThenInclude(l => l.bookCopy)
               .ToListAsync();

            return Ok(fines);
        }
        // Case 6 - Get Fines for Current User
        [HttpGet("my-fines")]
        [Authorize]
        public async Task<IActionResult> GetMyFines()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);

            if (userIdClaim == null)
            {
                return Unauthorized("User ID was not found in the token.");
            }

            int userId = int.Parse(userIdClaim.Value);

            var fines = await _context.Fines
                .Include(f => f.loan)
                .Where(f => f.loan.UserID == userId)
                .ToListAsync();

            return Ok(fines);
        }
        // Case 7 - Get Unpaid Fines
        [HttpGet("unpaid")]
        [Authorize(Roles = "Librarian,Admin")]
        public async Task<IActionResult> GetUnpaidFines()
        {
            var fines = await _context.Fines
                .Where(f => f.Status == FinePaymentStatus.Unpaid)
                .ToListAsync();

            return Ok(fines);
        }
        // Case 8 - Calculate Total Unpaid Fines
        [HttpGet("total-unpaid")]
        [Authorize(Roles = "Librarian,Admin")]
        public async Task<IActionResult> GetTotalUnpaidFines()
        {
            var totalUnpaid = await _context.Fines
                .Where(f => f.Status == FinePaymentStatus.Unpaid)
                .SumAsync(f => f.FineAmount);

            return Ok(new
            {
                TotalUnpaid = totalUnpaid
            });
        }
    }
}