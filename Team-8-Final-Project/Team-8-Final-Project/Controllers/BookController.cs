using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Team_8_Final_Project.Models;
namespace Team_8_Final_Project.Controllers
{
    [ApiController]
    [Route("Book")]
    public class BookController : ControllerBase
    {
        private ProjectContext context;
        public BookController(ProjectContext _context)
        {
            context = _context;
        }

        // Add a new Book
        [HttpPost("AddBook")]
        [Authorize(Roles = "Librarian,Admin")]
        public IActionResult AddBook(Book b)
        {
            context.books.Add(b);
            context.SaveChanges();

            return Ok(b);
        }

        // Full Update of a Book
        [HttpPut("UpdateBook/{id}")]
        [Authorize(Roles = "Librarian,Admin")]
        public IActionResult UpdateBook(int id, Book newBook)
        {
            Book b = context.books.FirstOrDefault(b => b.BookId == id);

            if (b == null)
            {
                return NotFound("Book not found.");
            }

            b.BookTitle = newBook.BookTitle;
            b.ISBN = newBook.ISBN;
            b.Year = newBook.Year;

            b.BookLanguage = newBook.BookLanguage;
            b.BookEdition = newBook.BookEdition;
            b.Authors = newBook.Authors;

            b.PublisherId = newBook.PublisherId;
            b.CategoryId = newBook.CategoryId;

            context.SaveChanges();

            return Ok(newBook);
        }

        // Reassign a Book to a different Publisher
        [HttpPatch("ReassignPublisher")]
        [Authorize(Roles = "Librarian,Admin")]
        public IActionResult ReassignPublisher(int id, int newPublisherId)
        {
            Book b = context.books.FirstOrDefault(b => b.BookId == id);

            if (b == null)
            {
                return NotFound("Book not found.");
            }

            b.PublisherId = newPublisherId;
            context.SaveChanges();

            return Ok(b);
        }


        // Delete a Book
        [HttpDelete("DeleteBook")]
        [Authorize(Roles = "Admin")]
        public IActionResult DeleteBook(int id)
        {
            Book b = context.books.FirstOrDefault(b => b.BookId == id);

            if (b == null)
            {
                return NotFound("Book not found.");
            }

            // Check if the book has any copies
            bool hasCopies = context.bookCopies.Any(bc => bc.BookId == id);

            if (hasCopies)
            {
                return BadRequest("This book cannot be deleted because it has book copies.");
            }

            context.books.Remove(b);
            context.SaveChanges();

            return Ok(b);
        }

        // Get all Books
        [HttpGet("GetAllBooks")]
        [Authorize]
        public IActionResult GetAllBooks()
        {
            List<Book> books = context.books.Include(b => b.Authors)
                                            .Include(b => b.Category)
                                            .Include(b => b.Publisher)
                                            .ToList();
            return Ok(books);

        }

        // Search Book by Title
        [HttpGet("SearchBook")]
        [Authorize]
        public IActionResult SearchBook(string title)
        {
            List<Book> books = context.books.Include(b => b.Authors)
                                            .Include(b => b.Category)
                                            .Include(b => b.Publisher)
                                            .Where(b => b.BookTitle.Contains(title))
                                            .ToList();

            if (books.Count == 0)
            {
                return NotFound("No books found with that title.");
            }

            return Ok(books);
        }

    }

}
