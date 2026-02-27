using Project1.Models.ENums;

namespace Project1.Models;

public class TaskItem {
    public long Id { get; set; }
    public required string Description { get; set; }
    public Priority Priority { get; set; }
    public Status Status { get; set; } = Status.NotStarted;
    public bool Completed { get; set; } = false;
    public DateTime CreatedAt { get; set; }
} 
