using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Team_8_Final_Project.Models
{
    public enum FinePaymentStatus
    {
        Paid,
        Unpaid,
        Dismissed
    }
    public class Fine
    {
        [Key]
        public int FineId { get; set; }

        [Required]
        public decimal FineAmount { get; set; }

        [Required]
        public FinePaymentStatus Status { get; set; }

        [Required]
        public DateTime FineIssueDate { get; set; }


        [ForeignKey("Loan")]
        public int LoanId { get; set; }
        [JsonIgnore]
        public Loan Loan { get; set; }
    }
}