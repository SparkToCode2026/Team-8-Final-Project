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

        public class CreateEventDto
        {
            public string EventName { get; set; }
            public DateTime EventDate { get; set; }
            public string EventLocation { get; set; }
            public string EventDescription { get; set; }
            public int EventMaxCap { get; set; }
        }

        public class UpdateEventDto
        {
            public string EventName { get; set; }
            public DateTime EventDate { get; set; }
            public string EventLocation { get; set; }
            public string EventDescription { get; set; }
            public int EventMaxCap { get; set; }
        }

        // Add a new event
        [HttpPost("AddEvent")]
        [Authorize(Roles = "Admin, Librarian")]
        public IActionResult AddEvent(CreateEventDto dto)
        {
            var newEvent = new Event
            {
                EventName = dto.EventName,
                EventDate = dto.EventDate,
                EventLocation = dto.EventLocation,
                EventDescription = dto.EventDescription,
                EventMaxCap = dto.EventMaxCap
            };

            context.Events.Add(newEvent);
            context.SaveChanges();

            return Ok(newEvent);
        }

        // Update an existing event (full update)
        [HttpPut("UpdateEvent")]
        [Authorize(Roles = "Admin, Librarian")]
        public IActionResult UpdateEvent(int id, UpdateEventDto dto)
        {
            Event existingEvent = context.Events.FirstOrDefault(e => e.EventId == id);

            if (existingEvent == null)
            {
                return NotFound("Event not found.");
            }

            existingEvent.EventName = dto.EventName;
            existingEvent.EventDate = dto.EventDate;
            existingEvent.EventLocation = dto.EventLocation;
            existingEvent.EventDescription = dto.EventDescription;
            existingEvent.EventMaxCap = dto.EventMaxCap;

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