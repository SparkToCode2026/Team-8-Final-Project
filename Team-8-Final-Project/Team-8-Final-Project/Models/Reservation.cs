namespace Team_8_Final_Project.Models
{
    public class Reservation
    {
        public int ReservationId { get; set; }
        public int BookCopyId { get; set; }
        public int UserId { get; set; }
        public string ReservationDate { get; set; }
        public string ReservationStatus { get; set; }
    }
}
