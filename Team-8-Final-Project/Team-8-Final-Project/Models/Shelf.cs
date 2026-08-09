using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Team_8_Final_Project.Models;

public class Shelf
{
    [Key]
    [JsonIgnore]
    public int ShelfID { get; set; }

    public string ShelfCode { get; set; } = string.Empty;

    public string Section { get; set; } = string.Empty;

    public int FloorNumber { get; set; }
}
    public string ShelfCode { get; set; }
    public string Section { get; set; }
    public int FloorNumber { get; set; }

    public List<BookCopy> bookCopies { get; set; }
}
