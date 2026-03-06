using Project1.Models.ENums;
using TaskStatus = Project1.Models.ENums.TaskStatus;

namespace Project1.Models.ViewModels;

public class TaskDisplay
{
    public int Id { get; set; }
    public string Description { get; set; } = null!;
    public TaskPriority Priority { get; set; }
    public TaskStatus Status { get; set; }
    public DateOnly DueTo { get; set; }
    
    public static TaskDisplay FromTask(TaskItem task) => new TaskDisplay
    {
        Id = task.Id,
        Description = task.Description,
        Priority = task.Priority,
        Status = task.Status,
        DueTo = task.DueTo
    };

    public string ToMenuString()
    {
        string display = $"#{Id} {Description}\n";
        display += $"Priority: {Priority}\n";
        display += $"Status: {Status}\n";
        display += $"Due To: {DueTo:dd-MM-yyyy}\n";
        
        return display;
    }
}