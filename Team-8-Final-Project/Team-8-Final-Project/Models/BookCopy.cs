using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

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
        [JsonIgnore]
        public int BookCopyId { get; set; }
        [Required]
        public string Barcode { get; set; }
        public ConditionStatus Condition { get; set; }
        public AvailabilityStatus AvailabilityStatus { get; set; }
        [Required]
        public decimal CopyPrice { get; set; }


        [ForeignKey("Book")]
        public int BookId { get; set; }
        public Book Book { get; set; }

        public List<Loan> Loans { get; set; }

        [ForeignKey("Shelf")]
        public int ShelfId { get; set; }
        public Shelf Shelf { get; set; }

    }
}
