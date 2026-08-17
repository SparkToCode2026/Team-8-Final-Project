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
        public int UserId { get; set; }
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required]
        [EmailAddress]
        public string UserEmail { get; set; }
        [Required]
        public string UserPhoneNo { get; set; }
        public DateTime DOB { get; set; }
        public UserRole Role { get; set; } = UserRole.Member;
        [Required]
        public string PasswordHash { get; set; }
        // For Forget Password
        public string? PasswordResetToken { get; set; }
        public DateTime? PasswordResetTokenExpiry { get; set; }


        [JsonIgnore]
        public List<Loan> Loans { get; set; }
        [JsonIgnore]
        public List<Reservation> Reservations { get; set; }
        [JsonIgnore]
        public List<Event> Events { get; set; }
    }
}