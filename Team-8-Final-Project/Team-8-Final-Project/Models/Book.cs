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

        [ForeignKey("Publisher")]
        public int PublisherId { get; set; }
        public Publisher Publisher { get; set; }

        [ForeignKey("Category")]
        public int CategoryId { get; set; }
        public Category Category { get; set; }

        public List<BookCopy> BookCopies { get; set; }

        public List<Author> Authors { get; set; }

        public List<Review> Reviews { get; set; } 

        public List<Reservation> Reservations { get; set; }

    }
}
