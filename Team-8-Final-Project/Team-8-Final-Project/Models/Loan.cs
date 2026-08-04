namespace Team_8_Final_Project.Models
{
    public class Loan
    {
        public int LoanId { get; set; }
        public int BookCopyId { get; set; }
        public int UserId { get; set; }
        public string LoanStartDate { get; set; }
        public string LoanEndDate { get; set; }
        public string LoanReturnDate { get; set; }

    }
}
