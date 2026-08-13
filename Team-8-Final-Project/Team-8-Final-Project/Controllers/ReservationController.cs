using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using Team_8_Final_Project.Models;

namespace Team_8_Final_Project.Controllers
{
    [ApiController]
    [Route("Reservation")]
    public class ReservationController : ControllerBase
    {


        private ProjectContext context;

        public ReservationController(ProjectContext _context)
        {
            context = _context;
        }


        public class CreateReservationDto
        {
            [Required]
            public DateTime ReservationDate { get; set; }

            [Required]
            public int BookId { get; set; }

            [Required]
            public int UserId { get; set; }
        }

        public class UpdateReservationStatusDto
        {
            [Required]
            public int ReservationId { get; set; }

            [Required]
            public ReservationStatus NewStatus { get; set; }
        }

        [HttpPost("AddReservation")]
        public IActionResult AddReservation(CreateReservationDto dto)
        {
            var reservation = new Reservation
            {
                ReservationDate = DateTime.Now,
                Status = ReservationStatus.Active,
                BookId = dto.BookId,
                UserId = dto.UserId
            };

            context.Reservations.Add(reservation);
            context.SaveChanges();
            return Ok(reservation);
        }

        [HttpPatch("UpdateReservationStatus")]
        public IActionResult UpdateReservationStatus(int reservationId, ReservationStatus newStatus)
        {
            var reservation = context.Reservations.Find(reservationId);
            if (reservation == null)
            {
                return NotFound();
            }
            reservation.Status = newStatus;
            context.SaveChanges();
            return Ok(reservation);
        }










    }
}
