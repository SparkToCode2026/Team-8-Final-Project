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
        public ReservationStatus status { get; set; }

        [ForeignKey("bookCopy")]
        public int BookCopyId { get; set; }
        public BookCopy bookCopy { get; set; }

        [ForeignKey("user")]
        public int UserId { get; set; }
        public User user { get; set; }
    }
}
