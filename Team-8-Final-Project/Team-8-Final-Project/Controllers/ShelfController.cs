using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Team_8_Final_Project.Models;

namespace Team_8_Final_Project.Controllers;

public class CreateShelfDto
{
    public string ShelfCode { get; set; }
    public string Section { get; set; }
    public int FloorNumber { get; set; }
}

public class UpdateShelfDto
{
    public string ShelfCode { get; set; }
    public string Section { get; set; }
    public int FloorNumber { get; set; }
}

[ApiController]
[Route("api/[controller]")]
public class ShelfController : ControllerBase
{
    private readonly ProjectContext _context;

    public ShelfController(ProjectContext context)
    {
        _context = context;
    }

    // Staff place/organize shelves — members don't create them.
    [HttpPost]
    [Authorize(Roles = "Librarian,Admin")]
    public async Task<IActionResult> Create(CreateShelfDto dto)
    {
        var shelf = new Shelf
        {
            ShelfCode = dto.ShelfCode,
            Section = dto.Section,
            FloorNumber = dto.FloorNumber
        };

        _context.Shelves.Add(shelf);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = shelf.ShelfId }, shelf);
    }

    [HttpPut("{id}")]
    [Authorize(Roles = "Librarian,Admin")]
    public async Task<IActionResult> Update(int id, UpdateShelfDto dto)
    {
        var shelf = await _context.Shelves.FindAsync(id);
        if (shelf == null) return NotFound();

        shelf.ShelfCode = dto.ShelfCode;
        shelf.Section = dto.Section;
        shelf.FloorNumber = dto.FloorNumber;

        await _context.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id}")]
    [Authorize(Roles = "Librarian,Admin")]
    public async Task<IActionResult> Delete(int id)
    {
        var shelf = await _context.Shelves.FindAsync(id);
        if (shelf == null) return NotFound();

        _context.Shelves.Remove(shelf);
        await _context.SaveChangesAsync();
        return NoContent();
    }

    // Anyone logged in can browse shelves — useful for finding where a book physically lives.
    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetAll()
    {
        var shelves = await _context.Shelves.ToListAsync();
        return Ok(shelves);
    }

    [HttpGet("{id}")]
    [Authorize]
    public async Task<IActionResult> GetById(int id)
    {
        var shelf = await _context.Shelves.FindAsync(id);
        if (shelf == null) return NotFound();
        return Ok(shelf);
    }

    [HttpGet("include")]
    [Authorize]
    public async Task<IActionResult> GetAllWithCopies()
    {
        var shelves = await _context.Shelves
            .Include(s => s.BookCopies)
            .ToListAsync();

        return Ok(shelves);
    }

    [HttpGet("sorted")]
    [Authorize]
    public async Task<IActionResult> GetShelvesOrdered()
    {
        var shelves = await _context.Shelves
            .OrderBy(s => s.FloorNumber)
            .ThenBy(s => s.Section)
            .ToListAsync();

        return Ok(shelves);
    }
}
