using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Team_8_Final_Project.Models;

public enum EventStatus
{
    Upcoming,
    Ongoing,
    Completed,
    Cancelled
}

public class Event
{
    [Key]
    public int EventId { get; set; }
    [Required]
    public string EventName { get; set; }
    [Required]
    public DateTime EventDate { get; set; }
    [Required]
    public string EventLocation { get; set; }
    public string EventDescription { get; set; }
    public int EventMaxCap { get; set; }
    public EventStatus Status { get; set; } = EventStatus.Upcoming;



    public List<User> Users { get; set; }
}