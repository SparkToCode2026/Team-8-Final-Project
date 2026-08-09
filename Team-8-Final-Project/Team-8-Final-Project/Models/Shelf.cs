using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Team_8_Final_Project.Models;

public class Shelf
{
    [Key]
    [JsonIgnore]
    public int ShelfId { get; set; }
    [Required]
    public string ShelfCode { get; set; }
    public string Section { get; set; }
    public int FloorNumber { get; set; }


    public List<BookCopy> bookCopies { get; set; }
}
