using Project1.Models.ENums;
using TaskStatus = Project1.Models.ENums.TaskStatus;

namespace Project1.Models.ViewModels;

public class CreateTaskModel
{
    public string Description { get; set; }
    public TaskPriority Priority { get; set; }
    public TaskStatus Status { get; set; }
    public DateOnly DueTo { get; set; }
    public int? AssignedTo { get; set; }
    public string? AssigneeName { get; set; } = "Unassigned";
    
    public CreateTaskModel()
    {
        Description = "";
        Priority = TaskPriority.Medium;
        Status = TaskStatus.NotStarted;
        DueTo = DateOnly.FromDateTime(DateTime.Today);
    }
}