using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Team_8_Final_Project.Models
{
    public class Fine
    {
        [Key]
        public int FineID { get; set; }

        [Required]
        public decimal FineAmount { get; set; }

        [Required]
        [StringLength(20)]
        public string FinePaymentStatus { get; set; } = string.Empty;

        [Required]
        public DateTime FineIssueDate { get; set; }

        // Foreign Key
        [Required]
        public int LoanID { get; set; }

        // Navigation Property
        [ForeignKey(nameof(LoanID))]
        public Loan? Loan { get; set; }
    }
}