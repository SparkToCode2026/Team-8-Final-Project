using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
namespace Team_8_Final_Project.Models
{
    public class Publisher
    {
        [Key]
        public int PublisherId { get; set; }

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
        [JsonIgnore]
        public List<Book> Books { get; set; }
    }
}

