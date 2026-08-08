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
        public string ReservationDate { get; set; }
        public string ReservationStatus { get; set; }

        [ForeignKey("BookCopyId")]
        public int BookCopyId { get; set; }
        public BookCopy BookCopy { get; set; }

        [ForeignKey("UserId")]
        public int UserID { get; set; }
        public User User { get; set; }
    }
}
