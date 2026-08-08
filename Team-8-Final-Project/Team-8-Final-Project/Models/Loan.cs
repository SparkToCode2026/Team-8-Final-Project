using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Team_8_Final_Project.Models
{
    public enum LoanStatus
    {
        Active,
        Returned,
        Overdue
    }

    public class Loan
    {
        [Key]
        [JsonIgnore]
        public int LoanId { get; set; }
        public string LoanStartDate { get; set; }
        public string LoanEndDate { get; set; }
        public string LoanReturnDate { get; set; }
        public LoanStatus  loanStatus { get; set; }  


        [ForeignKey("bookCopy")]
        public int BookCopyId { get; set; }
        public BookCopy bookCopy { get; set; }

        [ForeignKey("user")]
        public int UserID { get; set; }
        public User user { get; set; }

    }
}
