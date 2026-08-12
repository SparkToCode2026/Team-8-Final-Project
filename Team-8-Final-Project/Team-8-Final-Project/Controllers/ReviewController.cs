using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Team_8_Final_Project.Models;

namespace Team_8_Final_Project.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReviewController : ControllerBase
    {
        private readonly ProjectContext _context;

        public ReviewController(ProjectContext context)
        {
            _context = context;
        }
        // Case 1 - Submit Book Review
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateReview(Review review)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            review.ReviewDate = DateTime.Now;

            _context.Reviews.Add(review);
            await _context.SaveChangesAsync();

            return Ok(review);
        }
        // Case 2 - Edit Full Review
        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> UpdateReview(int id, Review updatedReview)
        {
            var review = await _context.Reviews.FindAsync(id);

            if (review == null)
            {
                return NotFound();
            }

            review.Rating = updatedReview.Rating;
            review.Comment = updatedReview.Comment;

            await _context.SaveChangesAsync();

            return Ok(review);
        }
        // Case 3 - Update Review Comment Only
        [HttpPatch("{id}/comment")]
        [Authorize]
        public async Task<IActionResult> UpdateReviewComment(int id, string comment)
        {
            var review = await _context.Reviews.FindAsync(id);

            if (review == null)
            {
                return NotFound();
            }

            review.Comment = comment;

            await _context.SaveChangesAsync();

            return Ok(review);
        }
        // Case 4 - Delete Review
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteReview(int id)
        {
            var review = await _context.Reviews.FindAsync(id);

            if (review == null)
            {
                return NotFound();
            }

            _context.Reviews.Remove(review);
            await _context.SaveChangesAsync();

            return Ok("Review deleted successfully.");
        }
        // Case 5 - Get Reviews for a Specific Book with User Details
        [HttpGet("book/{bookId}")]
        public async Task<IActionResult> GetReviewsByBook(int bookId)
        {
            var reviews = await _context.Reviews
                .Include(r => r.user)
                .Where(r => r.BookId == bookId)
                .ToListAsync();

            return Ok(reviews);
        }
        // Case 6 - Get Review by ID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetReviewById(int id)
        {
            var review = await _context.Reviews
                .Include(r => r.user)
                .Include(r => r.book)
                .FirstOrDefaultAsync(r => r.ReviewId == id);

            if (review == null)
            {
                return NotFound();
            }

            return Ok(review);
        }
        // Case 7 - Filter High Rating Reviews
        [HttpGet("filter")]
        public async Task<IActionResult> FilterHighRatings(int bookId)
        {
            var reviews = await _context.Reviews
                .Where(r => r.BookId == bookId && r.Rating >= 4)
                .ToListAsync();

            return Ok(reviews);
        }
    }
}