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

        public class UpdateReservationDto
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

        [HttpPut("UpdateReservation")]
        public IActionResult UpdateReservation(int id, UpdateReservationDto dto)
        {
            Reservation r = context.Reservations.FirstOrDefault(r => r.ReservationId == id);
            if (r == null) return NotFound("Reservation not found");

            r.ReservationDate = dto.ReservationDate;
            r.BookId = dto.BookId;
            r.UserId = dto.UserId;

            context.SaveChanges();
            return Ok(r);
        }

        [HttpPatch("UpdateReservationStatus")]
        public IActionResult UpdateReservationStatus(int reservationId, UpdateReservationStatusDto newStatus)
        {
            var reservation = context.Reservations.Find(reservationId);
            if (reservation == null)
            {
                return NotFound();
            }
            reservation.Status = newStatus.NewStatus;
            context.SaveChanges();
            return Ok(reservation);
        }

        [HttpDelete("RemoveReservation")]
        public IActionResult RemoveReservation(int id)
        {
            Reservation r = context.Reservations.FirstOrDefault(r => r.ReservationId == id);
            if (r == null)
            {
                return NotFound("Reservation not found");
            }
            context.Reservations.Remove(r);
            context.SaveChanges();
            return Ok("Reservation removed successfully");
        }

        [HttpGet("GetAllReservation")]
        public IActionResult GetAllReservation()
        {
            List<Reservation> reservations = context.Reservations.ToList();
            return Ok(reservations);
        }

        [HttpGet("GetReservation")]
        public IActionResult GetReservation(int id)
        {
            Reservation r = context.Reservations.FirstOrDefault(r => r.ReservationId == id);
            if (r == null)
            {
                return NotFound("Reservation not found");
            }
            return Ok(r);
        }




    }
}
