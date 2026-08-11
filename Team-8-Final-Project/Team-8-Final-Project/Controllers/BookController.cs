using Microsoft.AspNetCore.Mvc;
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
        public IActionResult AddBook(Book b)
        {
            context.books.Add(b);
            context.SaveChanges();

            return Ok(b);
        }

        // Full Update of a Book
        [HttpPut("UpdateBook/{id}")]
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

    }

}
