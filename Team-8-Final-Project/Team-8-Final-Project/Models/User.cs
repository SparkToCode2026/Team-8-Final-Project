using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Team_8_Final_Project.Models
{
    public class User
    {
        [Key]
        [JsonIgnore]
        public int UserID { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string UserEmail { get; set; } = string.Empty;
        public string UserPhoneNo { get; set; } = string.Empty;
        public DateTime DOB { get; set; }
        public string Role { get; set; } = "Member";
        public string PasswordHash { get; set; } = string.Empty;
    }
}