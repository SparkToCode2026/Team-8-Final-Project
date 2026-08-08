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
        public int LoanId { get; set; }
        public string LoanStartDate { get; set; }
        public string LoanEndDate { get; set; }
        public string LoanReturnDate { get; set; }
        public string LoanStatus { get; set; }


        [ForeignKey("BookCopyId")]
        public int BookCopyId { get; set; }
        public BookCopy BookCopy { get; set; }

        [ForeignKey("UserId")]
        public int UserId { get; set; }

    }
}
