using System;
using System.ComponentModel.DataAnnotations;
﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Team_8_Final_Project.Models
{
    public class Loan
    {
        [Key]
        [JsonIgnore]
        public int LoanID { get; set; }

        [Required]
        public DateTime LoanStartDate { get; set; }

        [Required]
        public DateTime LoanDueDate { get; set; }

        public DateTime? LoanReturnDate { get; set; }

        [Required]
        [StringLength(20)]
        public string LoanStatus { get; set; } = string.Empty;
        public int LoanId { get; set; }
        public string LoanStartDate { get; set; }
        public string LoanEndDate { get; set; }
        public string LoanReturnDate { get; set; }
        public LoanStatus  loanStatus { get; set; }  

        // Foreign Key - BookCopy
        [Required]
        public int BookCopyID { get; set; }

        [ForeignKey(nameof(BookCopyID))]
        public BookCopy? BookCopy { get; set; }

        // Foreign Key - User
        [Required]
        [ForeignKey("bookCopy")]
        public int BookCopyId { get; set; }
        public BookCopy bookCopy { get; set; }

        [ForeignKey("user")]
        public int UserID { get; set; }

        [ForeignKey(nameof(UserID))]
        public User? User { get; set; }
    }
}