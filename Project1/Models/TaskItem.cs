using Project1.Models.ENums;
using TaskStatus = Project1.Models.ENums.TaskStatus;

namespace Project1.Models;

public class TaskItem {
    public long Id { get; set; }
    public required string Description { get; set; }
    public TaskPriority TaskPriority { get; set; }
    public TaskStatus TaskStatus { get; set; } = TaskStatus.NotStarted;
    public bool Completed { get; set; } = false;
    public DateTime CreatedAt { get; set; }
} 
