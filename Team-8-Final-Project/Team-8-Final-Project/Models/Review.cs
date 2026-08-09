using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Team_8_Final_Project.Models
{
    public class Review
    {
        [Key]
        public int ReviewID { get; set; }

        [Required]
        public DateTime ReviewDate { get; set; }

        [Required]
        [Range(1, 5)]
        public int Rating { get; set; }

        [StringLength(500)]
        public string? Comment { get; set; }

        // Foreign Keys
        [Required]
        public int BookID { get; set; }

        [Required]
        public int UserID { get; set; }

        // Navigation Properties
        [ForeignKey(nameof(BookID))]
        public Book? Book { get; set; }

        [ForeignKey(nameof(UserID))]
        public User? User { get; set; }
    }
}