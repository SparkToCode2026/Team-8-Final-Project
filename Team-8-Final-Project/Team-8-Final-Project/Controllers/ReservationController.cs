using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

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
    }

    public class CreateReservationDto
    {
        [Required]
        public DateTime ReservationDate { get; set; }

        [Required]
        public string ReservationStatus { get; set; }

    }

}
