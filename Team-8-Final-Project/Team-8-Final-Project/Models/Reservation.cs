using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Team_8_Final_Project.Models
{
    public enum ReservationStatus
    {
        Active,
        Cancelled,
        Completed
    }
    public class Reservation
    {
        [Key]
        [JsonIgnore]
        public int ReservationId { get; set; }
        public DateTime ReservationDate { get; set; }
        public ReservationStatus Status { get; set; }

        [ForeignKey("book")]
        public int BookId { get; set; }
        public Book Book { get; set; }

        [ForeignKey("user")]
        public int UserId { get; set; }
        public User User { get; set; }
    }
}
