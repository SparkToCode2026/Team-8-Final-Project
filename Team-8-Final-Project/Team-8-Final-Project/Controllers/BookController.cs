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
            context.Books.Add(b);
            context.SaveChanges();

            return Ok(b);
        }

        // Full Update of a Book
        [HttpPut("UpdateBook/{id}")]
        [Authorize(Roles = "Librarian,Admin")]
        public IActionResult UpdateBook(int id, Book newBook)
        {
            Book b = context.Books.FirstOrDefault(b => b.BookId == id);

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
            Book b = context.Books.FirstOrDefault(b => b.BookId == id);

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
            Book b = context.Books.FirstOrDefault(b => b.BookId == id);

            if (b == null)
            {
                return NotFound("Book not found.");
            }

            // Check if the book has any copies
            bool hasCopies = context.BookCopies.Any(bc => bc.BookId == id);

            if (hasCopies)
            {
                return BadRequest("This book cannot be deleted because it has book copies.");
            }

            context.Books.Remove(b);
            context.SaveChanges();

            return Ok(b);
        }

        // Get all Books
        [HttpGet("GetAllBooks")]
        [Authorize]
        public IActionResult GetAllBooks()
        {
            List<Book> books = context.Books.Include(b => b.Authors)
                                            .Include(b => b.Category)
                                            .Include(b => b.Publisher)
                                            .ToList();
            return Ok(books);

        }

        // Search Book by Title and show available book copies
        [HttpGet("SearchBook")]
        [Authorize]
        public IActionResult SearchBook(string title)
        {
            List<Book> books = context.Books.Include(b => b.Authors)
                                            .Include(b => b.Category)
                                            .Include(b => b.Publisher)
                                            .Include(b => b.BookCopies)
                                            .Where(b => b.BookTitle.Contains(title))
                                            .ToList();

            if (books.Count == 0)
            {
                return NotFound("No books found with that title.");
            }

            List<object> results = new List<object>();

            foreach (Book book in books)
            {
                int availableCopies = book.BookCopies.Count(bc => bc.AvailabilityStatus == AvailabilityStatus.Available);

                results.Add(new{ Book = book, AvailableCopies = availableCopies });
            }

            return Ok(results);
        }

        // Filter Books by Category, Language, and Year
        [HttpGet("FilterBooks")]
        [Authorize]
        public IActionResult FilterBooks(int? categoryId, string? language, int? year)
        {
            List<Book> books = context.Books.Include(b => b.Authors)
                                            .Include(b => b.Category)
                                            .Include(b => b.Publisher)
                                            .Where(b =>
                                                 (!categoryId.HasValue || b.CategoryId == categoryId) &&
                                                 (language == null || b.BookLanguage == language) &&
                                                 (!year.HasValue || b.Year == year))
                                            .ToList();

            if (books.Count == 0)
            {
                return NotFound("No books match the selected filters.");
            }

            return Ok(books);
        }

        // Sort Books
        [HttpGet("SortBooks")]
        [Authorize]
        public IActionResult SortBooks(string sortBy, string order)
        {
            List<Book> books = context.Books
                .Include(b => b.Authors)
                .Include(b => b.Category)
                .Include(b => b.Publisher)
                .ToList();

            if (sortBy == "title")
            {
                if (order == "asc")
                {
                    books = books.OrderBy(b => b.BookTitle).ToList();
                }
                else if (order == "desc")
                {
                    books = books.OrderByDescending(b => b.BookTitle).ToList();
                }
            }
            else if (sortBy == "year")
            {
                if (order == "asc")
                {
                    books = books.OrderBy(b => b.Year).ToList();
                }
                else if (order == "desc")
                {
                    books = books.OrderByDescending(b => b.Year).ToList();
                }
            }
            else
            {
                return BadRequest("Invalid sorting option.");
            }

            return Ok(books);
        }

    }
}
