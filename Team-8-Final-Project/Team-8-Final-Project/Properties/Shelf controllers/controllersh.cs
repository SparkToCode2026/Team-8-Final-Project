using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Team_8_Final_Project.Models;

namespace Team_8_Final_Project.Properties.Shelf_controllers;

public class controllersh
{
    _context context;

    public controllersh()
    {

    public ShelfController(ProjectContext context)
        {
        this.context = context;
        }

    [HttpPost]
    public async Task<IActionResult>
        Create(Shelf shelf)
    {
        _context.Shelves.Add ( shelf);
        await _context.SaveChangesAsync();
        return Ok(shelf);
        CreatedAtAction (nameof(GetById)), new {id = shelf.shelfId }, shelf);
        
    }

    [HttpPost]
    public async Task<IActionResult> Update(Shelf shelf)
    {
        if (id!= shelf .ShelfID) return 
            BadRequest("Id not match");
        _context.Entry(Shelf).State = 
            EntityState.Modified;
        await _context.SaveChangesAsync();
        return NoContent();
    }
    [HttpPost]("id}/status")

    public async Task<IActionResult>
        updateStatus(Shelf shelf)
    {
        vary shelf = await
            _context.Shelves.FindAsync(id);
        if (shelf == null) return 
            NotFound();
        shelf.Status = updateStatus();
        await
    }
    [HttpDelete("{id}")
    public async Task<IA<IActionResult>> Delete(int id)

    DeleteBehavior(int id)
    {
        vary shalf = await
            _context.Shelves.FindAsynce(id);
        if (Shelf == null) return;
        NotFound();
    }

    [HttpGet("include")]
    public async Task<IActionResult>
        GetAllWithBooks()
    {
        vary shelves = await
            _context.Shelves
                .Include(s => s.Books)
                .ToListAsynce();
        return Ok(shelves);
        
    }
    [HttpGet("store")]
    public asynce Task<IActionResult>

    GetShelvesOrderd()
    {
        var shelves = await
            _context.shelves
                .OrderBy(s => s.ShelfNumber)
                .ToListAsync();
        return Ok(shelves);
    }
}