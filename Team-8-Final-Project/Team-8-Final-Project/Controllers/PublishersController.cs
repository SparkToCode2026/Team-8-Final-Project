using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Team_8_Final_Project.Models;

namespace Team_8_Final_Project.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PublishersController : ControllerBase
    {
        private readonly ProjectContext _context;

        public PublishersController(ProjectContext context)
        {
            _context = context;
        }

        public class PublisherCreateDto
        {
            public string PublisherCode { get; set; }
            public string PublisherName { get; set; }
            public string PublisherAddress { get; set; }
            public string PublisherLandlineNo { get; set; }
            public string PublisherEmail { get; set; }
        }

        public class PublisherUpdateDto
        {
            public string PublisherName { get; set; }
            public string PublisherAddress { get; set; }
            public string PublisherLandlineNo { get; set; }
            public string PublisherEmail { get; set; }
        }

        [HttpPost]
        public async Task<ActionResult<Publisher>> CreatePublisher(PublisherCreateDto dto)
        {
            var publisher = new Publisher
            {
                PublisherCode = dto.PublisherCode,
                PublisherName = dto.PublisherName,
                PublisherAddress = dto.PublisherAddress,
                PublisherLandlineNo = dto.PublisherLandlineNo,
                PublisherEmail = dto.PublisherEmail
            };

            _context.Publishers.Add(publisher);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetPublisherById), new { id = publisher.PublisherId }, publisher);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePublisher(int id, PublisherUpdateDto dto)
        {
            var publisher = await _context.Publishers.FindAsync(id);
            if (publisher == null) return NotFound();

            publisher.PublisherName = dto.PublisherName;
            publisher.PublisherAddress = dto.PublisherAddress;
            publisher.PublisherLandlineNo = dto.PublisherLandlineNo;
            publisher.PublisherEmail = dto.PublisherEmail;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpPut("{id}/reassign-books/{newPublisherId}")]
        public async Task<IActionResult> ReassignBooks(int id, int newPublisherId)
        {
            var oldPublisher = await _context.Publishers.FindAsync(id);
            var newPublisher = await _context.Publishers.FindAsync(newPublisherId);
            if (oldPublisher == null || newPublisher == null) return NotFound();

            var booksToReassign = await _context.Books
                .Where(b => b.PublisherId == id)
                .ToListAsync();

            foreach (var book in booksToReassign)
            {
                book.PublisherId = newPublisherId;
            }

            await _context.SaveChangesAsync();
            return Ok(new { ReassignedCount = booksToReassign.Count });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePublisher(int id)
        {
            var publisher = await _context.Publishers
                .Include(p => p.Books)
                .FirstOrDefaultAsync(p => p.PublisherId == id);

            if (publisher == null) return NotFound();

            if (publisher.Books.Any())
                return Conflict("Cannot delete a publisher that still has books. Reassign books first.");

            _context.Publishers.Remove(publisher);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Publisher>>> GetAllPublishers()
        {
            var publishers = await _context.Publishers
                .Include(p => p.Books)
                .ToListAsync();

            return Ok(publishers);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Publisher>> GetPublisherById(int id)
        {
            var publisher = await _context.Publishers
                .Include(p => p.Books)
                .FirstOrDefaultAsync(p => p.PublisherId == id);

            if (publisher == null) return NotFound();
            return Ok(publisher);
        }

        [HttpGet("by-category/{categoryId}")]
        public async Task<ActionResult<IEnumerable<Publisher>>> GetPublishersByCategory(int categoryId)
        {
            var publishers = await _context.Publishers
                .Where(p => p.Books.Any(b => b.CategoryId == categoryId))
                .Include(p => p.Books)
                .ToListAsync();

            return Ok(publishers);
        }

        [HttpGet("by-book-count")]
        public async Task<ActionResult<IEnumerable<object>>> GetPublishersByBookCount()
        {
            var result = await _context.Publishers
                .Select(p => new
                {
                    p.PublisherId,
                    p.PublisherName,
                    BookCount = p.Books.Count
                })
                .OrderByDescending(p => p.BookCount)
                .ToListAsync();

            return Ok(result);
        }
    }
}

