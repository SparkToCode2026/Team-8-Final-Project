using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Team_8_Final_Project.Models;
namespace Team_8_Final_Project.Controllers
{
    [ApiController]
    [Route("BookCopy")]
    public class BookCopyController : ControllerBase
    {
        private ProjectContext context;
        public BookCopyController(ProjectContext _context)
        {
            context = _context;
        }

        public class CreateBookCopyDto
        {
            public string Barcode { get; set; }
            public ConditionStatus Condition { get; set; }
            public AvailabilityStatus AvailabilityStatus { get; set; }
            public decimal CopyPrice { get; set; }
            public int BookId { get; set; }
            public int ShelfId { get; set; }
        }

        public class UpdateBookCopyDto
        {
            public string Barcode { get; set; }
            public ConditionStatus Condition { get; set; }
            public AvailabilityStatus AvailabilityStatus { get; set; }
            public decimal CopyPrice { get; set; }
            public int ShelfId { get; set; }
        }

        
        // Add a new book copy
        [HttpPost("AddBookCopy")]
        [Authorize(Roles = "Admin, Librarian")]
        public IActionResult AddBookCopy(CreateBookCopyDto dto)
        {
            var bookCopy = new BookCopy
            {
                Barcode = dto.Barcode,
                Condition = dto.Condition,
                AvailabilityStatus = dto.AvailabilityStatus,
                CopyPrice = dto.CopyPrice,
                BookId = dto.BookId,
                ShelfId = dto.ShelfId
            };

            context.BookCopies.Add(bookCopy);
            context.SaveChanges();

            return Ok(bookCopy);
        }

        // Update an existing book copy
        [HttpPut("UpdateBookCopy")]
        [Authorize(Roles = "Admin, Librarian")]
        public IActionResult UpdateBookCopy(int id, UpdateBookCopyDto dto)
        {
            BookCopy existingBookCopy = context.BookCopies.FirstOrDefault(bc => bc.BookCopyId == id);

            if (existingBookCopy == null)
            {
                return NotFound("Book copy not found.");
            }

            existingBookCopy.Barcode = dto.Barcode;
            existingBookCopy.Condition = dto.Condition;
            existingBookCopy.AvailabilityStatus = dto.AvailabilityStatus;
            existingBookCopy.CopyPrice = dto.CopyPrice;
            existingBookCopy.ShelfId = dto.ShelfId;

            context.SaveChanges();

            return Ok(existingBookCopy);
        }

        // Update condition and/or availability status of a book copy
        [HttpPatch("UpdateBookCopyStatus")]
        [Authorize(Roles = "Admin, Librarian")]
        public IActionResult UpdateBookCopyStatus( int id, ConditionStatus? condition, AvailabilityStatus? availabilityStatus)
        {
            BookCopy existingBookCopy = context.BookCopies.FirstOrDefault(bc => bc.BookCopyId == id);

            if (existingBookCopy == null)
            {
                return NotFound("Book copy not found.");
            }

            if (condition == null && availabilityStatus == null)
            {
                return BadRequest("Please provide condition, availability status, or both.");
            }

            if (condition != null)
            {
                existingBookCopy.Condition = condition.Value;
            }

            if (availabilityStatus != null)
            {
                existingBookCopy.AvailabilityStatus = availabilityStatus.Value;
            }

            context.SaveChanges();

            return Ok(existingBookCopy);
        }

        // Delete a book copy
        [HttpDelete("DeleteBookCopy")]
        [Authorize(Roles = "Admin, Librarian")]
        public IActionResult DeleteBookCopy(int id)
        {
            BookCopy existingBookCopy = context.BookCopies.FirstOrDefault(bc => bc.BookCopyId == id);
            if (existingBookCopy == null)
            {
                return NotFound("Book copy not found.");
            }
            context.BookCopies.Remove(existingBookCopy);
            context.SaveChanges();

            return Ok("BookCopy with ID "+ id + "has been deleted.");
        }

        // Get all book copies
        [HttpGet("GetAllBookCopies")]
        [Authorize]
        public IActionResult GetAllBookCopies()
        {
            List<BookCopy> bookCopies = context.BookCopies.Include(bc => bc.Book)
                                                          .Include(bc => bc.Shelf)
                                                          .ToList();

            return Ok(bookCopies);
        }

        // Get a book copy by Barcode
        [HttpGet("GetBookCopyByBarcode")]
        [Authorize]
        public IActionResult GetBookCopyByBarcode(string barcode)
        {
            BookCopy bookCopy = context.BookCopies.FirstOrDefault(bc => bc.Barcode == barcode);

            if (bookCopy == null)
            {
                return NotFound("Book copy not found.");
            }

            return Ok(bookCopy);
        }

        // Filter by Availability Status
        [HttpGet("FilterBookCopiesByAvailabilityStatus")]
        [Authorize]
        public IActionResult FilterBookCopiesByAvailabilityStatus(AvailabilityStatus availabilityStatus)
        {
            List<BookCopy> bookCopies = context.BookCopies.Where(bc => bc.AvailabilityStatus == availabilityStatus)
                                                          .Include(bc => bc.Book)
                                                          .Include(bc => bc.Shelf)
                                                          .ToList();
            if (bookCopies.Count == 0)
            {
                return NotFound("No available book copies found.");
            }

            return Ok(bookCopies);
        }

        // Count Book Copies by Availability Status (3 of them)
        [HttpGet("GetCopyCountByStatus")]
        [Authorize(Roles = "Librarian,Admin")]
        public IActionResult GetCopyCountByStatus()
        {
            var copyCounts = context.BookCopies.GroupBy(bc => bc.AvailabilityStatus)
                                               .Select(g => new { Status = g.Key, TotalCopies = g.Count() })
                                               .ToList();

            return Ok(copyCounts);
        }

    }
}
