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
    }
}