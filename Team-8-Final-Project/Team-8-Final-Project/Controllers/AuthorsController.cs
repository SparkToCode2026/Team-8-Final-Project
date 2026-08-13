using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Team_8_Final_Project.Models;

namespace Team_8_Final_Project.Controllers
{
    
    [ApiController]
    [Route("Author")]
    public class AuthorsController : ControllerBase
    {
        private readonly ProjectContext context;

        public AuthorsController(ProjectContext _context)
        {
            context = _context;
        }

        public class CreateAuthorDto
        {
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string Email { get; set; }
            public string Biography { get; set; }
            public string Nationality { get; set; }
        }

        public class UpdateAuthorDto
        {
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string Email { get; set; }
            public string Biography { get; set; }
            public string Nationality { get; set; }
        }


        // Add a new author
        [HttpPost("AddAuthor")]
        [Authorize(Roles = "Admin, Librarian")]
        public IActionResult AddAuthor(CreateAuthorDto dto)
        {
            var author = new Author
            {
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Email = dto.Email,
                Biography = dto.Biography,
                Nationality = dto.Nationality
            };

            context.Authors.Add(author);
            context.SaveChanges();

            return Ok(author);
        }

        // Update an existing author (full update)
        [HttpPut("UpdateAuthor")]
        [Authorize(Roles = "Admin, Librarian")]
        public IActionResult UpdateAuthor(int id, UpdateAuthorDto dto)
        {
            Author existingAuthor = context.Authors.FirstOrDefault(a => a.AuthorId == id);

            if (existingAuthor == null)
            {
                return NotFound("Author not found.");
            }

            existingAuthor.FirstName = dto.FirstName;
            existingAuthor.LastName = dto.LastName;
            existingAuthor.Email = dto.Email;
            existingAuthor.Biography = dto.Biography;
            existingAuthor.Nationality = dto.Nationality;

            context.SaveChanges();

            return Ok(existingAuthor);
        }

        // Update only the email of an author
        [HttpPatch("UpdateAuthorEmail")]
        [Authorize(Roles = "Admin, Librarian")]
        public IActionResult UpdateAuthorEmail(int id, string newEmail)
        {
            Author existingAuthor = context.Authors.FirstOrDefault(a => a.AuthorId == id);

            if (existingAuthor == null)
            {
                return NotFound("Author not found.");
            }

            existingAuthor.Email = newEmail;
            context.SaveChanges();

            return Ok(existingAuthor);
        }

        // Delete an author
        [HttpDelete("DeleteAuthor")]
        [Authorize(Roles = "Admin, Librarian")]
        public IActionResult DeleteAuthor(int id)
        {
            Author existingAuthor = context.Authors.FirstOrDefault(a => a.AuthorId == id);
            if (existingAuthor == null)
            {
                return NotFound("Author not found.");
            }

            context.Authors.Remove(existingAuthor);
            context.SaveChanges();

            return Ok("Author with ID " + id + " has been deleted.");
        }

        // Get all authors, including the books they've written
        [HttpGet("GetAllAuthors")]
        [Authorize]
        public IActionResult GetAllAuthors()
        {
            List<Author> authors = context.Authors.Include(a => a.Books).ToList();

            return Ok(authors);
        }

        // Get a single author by Id
        [HttpGet("GetAuthorById")]
        [Authorize]
        public IActionResult GetAuthorById(int id)
        {
            Author author = context.Authors.Include(a => a.Books).FirstOrDefault(a => a.AuthorId == id);

            if (author == null)
            {
                return NotFound("Author not found.");
            }

            return Ok(author);
        }

        // 7. GET Filter: Filter authors by Nationality using LINQ (Case 7 - Where)
        [HttpGet("filter-by-nationality/{nationality}")]
        public async Task<ActionResult<IEnumerable<Author>>> GetAuthorsByNationality(string nationality)
        {
            return await _context.Authors
                .Where(a => a.Nationality.ToLower() == nationality.ToLower())
                .ToListAsync();
        }

        // 8. GET Aggregate/Sort: Get total authors count & sorted list (Case 8 - OrderBy & Count)
        [HttpGet("stats")]
        public async Task<IActionResult> GetAuthorStats()
        {
            var totalAuthors = await _context.Authors.CountAsync();
            var sortedAuthors = await _context.Authors
                .OrderBy(a => a.LastName)
                .Select(a => new { a.AuthorId, FullName = a.FirstName + " " + a.LastName, a.Nationality })
                .ToListAsync();

            return Ok(new
            {
                TotalCount = totalAuthors,
                AuthorsSorted = sortedAuthors
            });
        }
    }
}