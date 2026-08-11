using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Team_8_Final_Project.Models
{
    public class Book
    {
        [Key]
        [JsonIgnore]
        public int BookId {  get; set; }
        [Required]
        public string ISBN { get; set; }
        [Required]
        public string BookTitle { get; set; }
        public int BookEdition { get; set; }
        [Required]
        public string BookLanguage { get; set; }
        public int Year { get; set; }

        [ForeignKey("publisher")]
        public int PublisherID { get; set; }
        public Publisher publisher { get; set; }

        [ForeignKey("category")]
        public int CategoryID { get; set; }
        public Category category { get; set; }

        public List<BookCopy> bookCopies { get; set; }

        public List<Author> Authors { get; set; }

        public List<Review> Reviews { get; set; } 

        public List<Reservation> Reservations { get; set; }

    }
}
