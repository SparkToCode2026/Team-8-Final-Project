using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Team_8_Final_Project.Models
{
    public class Book
    {
        [Key]
        [JsonIgnore]
        public int BookId {  get; set; }
        [Required]
        public string IBSN { get; set; }
        [Required]
        public string BookTitle { get; set; }
        public int BookEdition { get; set; }
        [Required]
        public string BookLanguage { get; set; }
        public int Year { get; set; }


        public List<BookCopy> bookCopies { get; set; }

        public List<Author> Authors { get; set; }



    }
}
