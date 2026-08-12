using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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

        // Add a new book copy
        [HttpPost("AddBookCopy")]
        [Authorize(Roles = "Admin, Librarian")]
        public IActionResult AddBookCopy(BookCopy bookCopy)
        {
            context.BookCopies.Add(bookCopy);
            context.SaveChanges();

            return Ok(bookCopy);
        }

        // Update an existing book copy
        [HttpPut("UpdateBookCopy")]
        [Authorize(Roles = "Admin, Librarian")]
        public IActionResult UpdateBookCopy(int id, BookCopy bookCopy)
        {
            BookCopy existingBookCopy = context.BookCopies.FirstOrDefault(bc => bc.BookCopyId == id);

            if (existingBookCopy == null)
            {
                return NotFound("Book copy not found.");
            }

            existingBookCopy.Barcode = bookCopy.Barcode;
            existingBookCopy.Condition = bookCopy.Condition;
            existingBookCopy.AvailabilityStatus = bookCopy.AvailabilityStatus;
            existingBookCopy.CopyPrice = bookCopy.CopyPrice;
            existingBookCopy.ShelfId = bookCopy.ShelfId;

            context.SaveChanges();

            return Ok(existingBookCopy);
        }

    }
}
