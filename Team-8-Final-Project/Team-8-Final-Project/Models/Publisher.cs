using System.ComponentModel.DataAnnotations;

namespace Team_8_Final_Project.Models
{
    public class Publisher
    {
        [Key]
        public int PublisherID { get; set; }

        [Required]
        [MaxLength(50)]
        public string PublisherCode { get; set; }

        [Required]
        [MaxLength(150)]
        public string PublisherName { get; set; }

        [MaxLength(250)]
        public string PublisherAddress { get; set; }

        [MaxLength(30)]
        public string PublisherLandlineNo { get; set; }

        [MaxLength(150)]
        [EmailAddress]
        public string PublisherEmail { get; set; }

        public ICollection<Book> Books { get; set; } = new List<Book>();
    }
}

