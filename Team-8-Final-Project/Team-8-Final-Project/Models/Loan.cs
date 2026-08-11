using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Team_8_Final_Project.Models
{
    public enum LoanStatus
    {
        Active,
        Overdue,
        Returned
    }
    public class Loan
    {
        [Key]
        [JsonIgnore]
        public int LoanId { get; set; }

        [Required]
        public DateTime LoanStartDate { get; set; }

        [Required]
        public Double LoanAmount { get; set; }

        [Required]
        public DateTime LoanDueDate { get; set; }
        public DateTime LoanReturnDate { get; set; }
        public LoanStatus  loanStatus { get; set; }

        // Foreign Key - BookCopy
        [ForeignKey("bookCopy")]
        public int BookCopyId { get; set; }
        public BookCopy bookCopy { get; set; }

        // Foreign Key - User
        [ForeignKey("user")]
        public int UserID { get; set; }
        public User user { get; set; }
    }
}