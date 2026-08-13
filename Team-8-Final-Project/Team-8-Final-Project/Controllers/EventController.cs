namespace Team_8_Final_Project.Controllers;

public class EventController
{
    [HttpPost]
    public async Task<IActionResult> CreateEvent([FromBody] Event newEvent)
    {
        _context.Events.Add(newEvent);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetEventById), new { id = newEvent.Id }, newEvent);
    }
    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateEventName(int id, [FromBody] Event updatedEvent)
    {
        var eventItem = await _context.Events.FindAsync(id);
        if (eventItem == null) return NotFound

        eventItem.Name = updatedEvent.Name; 
        await _context.SaveChangesAsync();
        return Ok(eventItem);
    }
    public async Task<IActionResult> UpdateEventStatus(int id, [FromBody] string status)
    {
        var eventItem = await _context.Events.FindAsync(id);
        if (eventItem == null) return NotFound

        eventItem.Status = status; 
        await _context.SaveChangesAsync();
        return Ok(eventItem);
    }
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteEvent(int id)
    {
        var eventItem = await _context.Events.FindAsync(id);
        if (eventItem == null) return NotFound

        _context.Events.Remove(eventItem);
        await _context.SaveChangesAsync();
        return NoContent();
    }
    [HttpGet("include")]
    public async Task<IActionResult> GetAllEventsWithUsers()
    {
        var events = await _context.Events
            .Include(e => e.Users)
            .ToListAsync();
        return Ok(events);
    }
    [HttpGet("{id}")]
    public async Task<IActionResult> GetEventById(int id)
    {
        var eventItem = await _context.Events
            .Include(e => e.Users)
            .FirstOrDefaultAsync(e => e.Id == id);
        if (eventItem == null) return 
            NotFound();
        return Ok(eventItem);
}
    [HttpGet("store")]
    public async Task<IActionResult> GetSortedEvents([FromQuery] string sortBy = "Date")
    {
        var events = _context.Events.AsQueryable();

        events = sortBy.ToLower() switch
        {
            "name" => events.OrderBy(e => e.Name),
            "status" => events.OrderBy(e => e.Status),
            _ => events.OrderBy(e => e.Date)
        };

        return Ok(await events.ToListAsync());
    }
}