using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Team_8_Final_Project.Models;
namespace Team_8_Final_Project.Controllers
{
    [ApiController]
    [Route("Event")]
    public class EventController : ControllerBase
    {
        private ProjectContext context;
        public EventController(ProjectContext _context)
        {
            context = _context;
        }

        // Add a new event
        [HttpPost("AddEvent")]
        [Authorize(Roles = "Admin, Librarian")]
        public IActionResult AddEvent(Event newEvent)
        {
            context.Events.Add(newEvent);
            context.SaveChanges();

            return Ok(newEvent);
        }

        // Update an existing event (full update)
        [HttpPut("UpdateEvent")]
        [Authorize(Roles = "Admin, Librarian")]
        public IActionResult UpdateEvent(int id, Event updatedEvent)
        {
            Event existingEvent = context.Events.FirstOrDefault(e => e.EventId == id);

            if (existingEvent == null)
            {
                return NotFound("Event not found.");
            }

            existingEvent.EventName = updatedEvent.EventName;
            existingEvent.EventDate = updatedEvent.EventDate;
            existingEvent.EventLocation = updatedEvent.EventLocation;
            existingEvent.EventDescription = updatedEvent.EventDescription;
            existingEvent.EventMaxCap = updatedEvent.EventMaxCap;

            context.SaveChanges();

            return Ok(existingEvent);
        }

        // Update only the status of an event
        [HttpPatch("UpdateEventStatus")]
        [Authorize(Roles = "Admin, Librarian")]
        public IActionResult UpdateEventStatus(int id, EventStatus newStatus)
        {
            Event existingEvent = context.Events.FirstOrDefault(e => e.EventId == id);

            if (existingEvent == null)
            {
                return NotFound("Event not found.");
            }

            existingEvent.Status = newStatus;
            context.SaveChanges();

            return Ok(existingEvent);
        }

        // Delete an event
        [HttpDelete("DeleteEvent")]
        [Authorize(Roles = "Admin, Librarian")]
        public IActionResult DeleteEvent(int id)
        {
            Event existingEvent = context.Events.FirstOrDefault(e => e.EventId == id);
            if (existingEvent == null)
            {
                return NotFound("Event not found.");
            }
            context.Events.Remove(existingEvent);
            context.SaveChanges();

            return Ok("Event with ID " + id + " has been deleted.");
        }

        // Get all events, including the users registered for each one
        [HttpGet("GetAllEvents")]
        [Authorize]
        public IActionResult GetAllEvents()
        {
            List<Event> events = context.Events.Include(e => e.Users).ToList();

            return Ok(events);
        }

        // Get an event by its Id
        [HttpGet("GetEventById")]
        [Authorize]
        public IActionResult GetEventById(int id)
        {
            Event eventItem = context.Events.FirstOrDefault(e => e.EventId == id);

            if (eventItem == null)
            {
                return NotFound("Event not found.");
            }

            return Ok(eventItem);
        }

        // Filter events by status
        [HttpGet("FilterEventsByStatus")]
        [Authorize]
        public IActionResult FilterEventsByStatus(EventStatus status)
        {
            List<Event> events = context.Events.Where(e => e.Status == status).ToList();

            if (events.Count == 0)
            {
                return NotFound("No events found with that status.");
            }

            return Ok(events);
        }

        // Get all events sorted by date, soonest first
        [HttpGet("GetEventsSortedByDate")]
        [Authorize]
        public IActionResult GetEventsSortedByDate()
        {
            List<Event> events = context.Events.OrderBy(e => e.EventDate).ToList();

            return Ok(events);
        }
    }
}
