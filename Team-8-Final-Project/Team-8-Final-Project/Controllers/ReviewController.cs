using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Team_8_Final_Project.Models;

namespace Team_8_Final_Project.Controllers
{
    [ApiController]
    [Route("Review")]
    public class ReviewController : ControllerBase
    {
        private ProjectContext context;

        public ReviewController(ProjectContext _context)
        {
            context = _context;
        }

        public class CreateReviewDto
        {
            public int Rating { get; set; }
            public string Comment { get; set; }
            public int BookId { get; set; }
        }

        public class UpdateReviewDto
        {
            public int Rating { get; set; }
            public string Comment { get; set; }
        }

        // Reads the logged-in user's Id out of the JWT token attached to this request
        private int GetCurrentUserId()
        {
            string userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return int.Parse(userIdClaim);
        }

        // Submit a new book review - always saved under whoever is currently logged in
        [HttpPost("AddReview")]
        [Authorize]
        public IActionResult AddReview(CreateReviewDto dto)
        {
            var review = new Review
            {
                ReviewDate = DateTime.Now,
                Rating = dto.Rating,
                Comment = dto.Comment,
                BookId = dto.BookId,
                UserId = GetCurrentUserId()
            };

            context.Reviews.Add(review);
            context.SaveChanges();

            return Ok(review);
        }

        // Update an existing review (full update - rating and comment)
        // Only the review's original author, a Librarian, or an Admin may do this
        [HttpPut("UpdateReview")]
        [Authorize]
        public IActionResult UpdateReview(int id, UpdateReviewDto dto)
        {
            Review existingReview = context.Reviews.FirstOrDefault(r => r.ReviewId == id);

            if (existingReview == null)
            {
                return NotFound("Review not found.");
            }

            bool isOwner = existingReview.UserId == GetCurrentUserId();
            bool isStaff = User.IsInRole("Librarian") || User.IsInRole("Admin");

            if (!isOwner && !isStaff)
            {
                return Forbid();
            }

            existingReview.Rating = dto.Rating;
            existingReview.Comment = dto.Comment;

            context.SaveChanges();

            return Ok(existingReview);
        }

        // Update only the comment of a review
        // Only the review's original author, a Librarian, or an Admin may do this
        [HttpPatch("UpdateReviewComment")]
        [Authorize]
        public IActionResult UpdateReviewComment(int id, string comment)
        {
            Review existingReview = context.Reviews.FirstOrDefault(r => r.ReviewId == id);

            if (existingReview == null)
            {
                return NotFound("Review not found.");
            }

            bool isOwner = existingReview.UserId == GetCurrentUserId();
            bool isStaff = User.IsInRole("Librarian") || User.IsInRole("Admin");

            if (!isOwner && !isStaff)
            {
                return Forbid();
            }

            existingReview.Comment = comment;

            context.SaveChanges();

            return Ok(existingReview);
        }

        // Delete a review
        // Only the review's original author, a Librarian, or an Admin may do this
        [HttpDelete("DeleteReview")]
        [Authorize]
        public IActionResult DeleteReview(int id)
        {
            Review existingReview = context.Reviews.FirstOrDefault(r => r.ReviewId == id);

            if (existingReview == null)
            {
                return NotFound("Review not found.");
            }

            bool isOwner = existingReview.UserId == GetCurrentUserId();
            bool isStaff = User.IsInRole("Librarian") || User.IsInRole("Admin");

            if (!isOwner && !isStaff)
            {
                return Forbid();
            }

            context.Reviews.Remove(existingReview);
            context.SaveChanges();

            return Ok("Review deleted successfully.");
        }

        // Get all reviews for a specific book, including the reviewer's details
        [HttpGet("GetReviewsByBook")]
        [Authorize]
        public IActionResult GetReviewsByBook(int bookId)
        {
            List<Review> reviews = context.Reviews.Include(r => r.User)
                                                   .Where(r => r.BookId == bookId)
                                                   .ToList();

            return Ok(reviews);
        }

        // Get a single review by Id
        [HttpGet("GetReviewById")]
        [Authorize]
        public IActionResult GetReviewById(int id)
        {
            Review review = context.Reviews.Include(r => r.User)
                                            .Include(r => r.Book)
                                            .FirstOrDefault(r => r.ReviewId == id);

            if (review == null)
            {
                return NotFound("Review not found.");
            }

            return Ok(review);
        }

        // Filter a book's reviews down to high ratings (4 and up)
        [HttpGet("FilterHighRatingReviews")]
        [Authorize]
        public IActionResult FilterHighRatingReviews(int bookId)
        {
            List<Review> reviews = context.Reviews.Where(r => r.BookId == bookId && r.Rating >= 4).ToList();

            return Ok(reviews);
        }

        // Get the average rating for a book using LINQ's Average() method which is the average mean
        [HttpGet("GetAverageRating")]
        [Authorize]
        public IActionResult GetAverageRating(int bookId)
        {
            var reviews = context.Reviews.Where(r => r.BookId == bookId);

            if (!reviews.Any())
            {
                return NotFound("No reviews found for this book.");
            }

            double averageRating = reviews.Average(r => r.Rating);

            return Ok(new
            {
                BookId = bookId,
                AverageRating = averageRating
            });
        }
    }
}
