using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Team_8_Final_Project.Models
{
    public class Author
    {
        [Key]
        [JsonIgnore]
        public int AuthorId { get; set; }
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        [Required]
        public string Email { get; set; }
        public string Biography { get; set; }
        public string Nationality { get; set; }

        [JsonIgnore]
        public List<Book> Books { get; set; }
    }
}