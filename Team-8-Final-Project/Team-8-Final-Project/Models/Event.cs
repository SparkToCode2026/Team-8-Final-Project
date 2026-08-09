using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Team_8_Final_Project.Models;

public class Event
{
    [Key]
    [JsonIgnore]
    public int EventID { get; set; }
    public string EventName { get; set; }
    public DateTime EventDate{ get; set; }
    public string EventLocation { get; set; }
    public string EventDescription { get; set; }
    }
    

    public List<User> Users { get; set; }
}
