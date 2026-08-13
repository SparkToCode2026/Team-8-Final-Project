using System.ComponentModel.DataAnnotations;

namespace Team_8_Final_Project.Models
{
    public class Category
    {
        [Key]
        public int CategoryId { get; set; }

        [Required]
        [MaxLength(100)]
        public string CategoryName { get; set; }

        [MaxLength(500)]
        public string CategoryDescription { get; set; }

        public List<Book> Books { get; set; }
    }
}
