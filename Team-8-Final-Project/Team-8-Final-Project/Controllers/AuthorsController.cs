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

        // 2. PUT: Update full Author details (Case 2)
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateAuthor(int id, Author author)
        {
            if (id != author.AuthorId) return BadRequest("ID Mismatch");

            _context.Entry(author).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_context.Authors.Any(e => e.AuthorId == id)) return NotFound();
                throw;
            }

            return NoContent();
        }

        // 3. PATCH/PUT: Distinct update case - Update Email only (Case 3)
        [HttpPatch("{id}/email")]
        public async Task<IActionResult> UpdateAuthorEmail(int id, [FromBody] string newEmail)
        {
            var author = await _context.Authors.FindAsync(id);
            if (author == null) return NotFound();

            author.Email = newEmail;
            await _context.SaveChangesAsync();

            return Ok(author);
        }

        // 4. DELETE: Delete an Author (Case 4)
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAuthor(int id)
        {
            var author = await _context.Authors.FindAsync(id);
            if (author == null) return NotFound();

            _context.Authors.Remove(author);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Author deleted successfully" });
        }

        // 5. GET List: Get all authors with their related books (Case 5 - Include)
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Author>>> GetAllAuthors()
        {
            return await _context.Authors
                .Include(a => a.Books)
                .ToListAsync();
        }

        // 6. GET Find: Get single Author by Id (Case 6)
        [HttpGet("{id}")]
        public async Task<ActionResult<Author>> GetAuthorById(int id)
        {
            var author = await _context.Authors
                .Include(a => a.Books)
                .FirstOrDefaultAsync(a => a.AuthorId == id);

            if (author == null) return NotFound();

            return author;
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