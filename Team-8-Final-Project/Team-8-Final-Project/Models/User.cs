using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Team_8_Final_Project.Models
{
    public enum UserRole
    {
        Member,
        Librarian,
        Admin
    }
    public class User
    {
        [Key]
        [JsonIgnore]
        public int UserId { get; set; }
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required]
        public string UserEmail { get; set; }
        [Required]
        public string UserPhoneNo { get; set; }
        public DateTime DOB { get; set; }
        public UserRole Role { get; set; } = UserRole.Member;
        [Required]
        public string PasswordHash { get; set; }


        public List<Loan> Loans { get; set; }

        public List<Reservation> Reservations { get; set; }

        public List<Event> Events { get; set; }
    }
}