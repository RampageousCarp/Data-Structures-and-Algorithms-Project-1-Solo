using Project1.Models.ENums;
using TaskStatus = Project1.Models.ENums.TaskStatus;

namespace Project1.Models.ViewModels;

public class TaskTableView
{
    public int Id { get; set; }
    public string Description { get; set; } = null!;
    public TaskPriority Priority { get; set; }
    public TaskStatus Status { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateOnly DueTo { get; set; }
    
    public static TaskTableView FromTask(TaskItem task) => new TaskTableView
    {
        Id = task.Id,
        Description = task.Description,
        Priority = task.Priority,
        Status = task.Status,
        CreatedAt = task.CreatedAt,
        DueTo = task.DueTo
    };

    public string ToTableId() => $"[ID: {Id}]";

    public string ToTableDescription() => $"Desc: {Description}";
        
    public string ToTablePrio() => $"Prio: {Priority}";
    
    public string ToTableCreated() => $"Created: {CreatedAt:dd-MM-yyyy}";
    
    public string ToTableDueTo() => $"Due To: {DueTo:dd-MM-yyyy}";
    
}