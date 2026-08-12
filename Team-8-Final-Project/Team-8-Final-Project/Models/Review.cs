using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Team_8_Final_Project.Models
{
    public class Review
    {
        [Key]
        public int ReviewId { get; set; }

        [Required]
        public DateTime ReviewDate { get; set; }

        [Required]
        [Range(1, 5)]
        public int Rating { get; set; }

        public string Comment { get; set; }


        [ForeignKey("book")]
        public int BookId { get; set; }
        public Book Book { get; set; }

        [ForeignKey("user")]
        public int UserId { get; set; }
        public User User { get; set; }
    }
}