using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Team_8_Final_Project.Models;

public class Event
{
    [Key]
    [JsonIgnore]
    public int EventID { get; set; }
    [Required]
    public string EventName { get; set; }
    [Required]
    public DateTime EventDate{ get; set; }
    [Required]
    public string EventLocation { get; set; }
    public string EventDescription { get; set; }
    public int EventMaxCap { get; set; }
    
    

    public List<User> Users { get; set; }
}
