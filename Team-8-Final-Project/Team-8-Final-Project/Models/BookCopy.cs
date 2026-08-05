using System.ComponentModel.DataAnnotations;

namespace Team_8_Final_Project.Models
{
    public enum ConditionStatus
    {
        New,
        Good,
        Fair,
        Poor
    }

    public enum AvailabilityStatus
    {
        Available,
        OnLoan,
        Reserved
    }

    public class BookCopy
    {
        [Key]
        public int BookCopyId { get; set; }
        public string Barcode { get; set; }
        public ConditionStatus Condition { get; set; }
        public AvailabilityStatus AvailabilityStatus { get; set; }

    }
}
