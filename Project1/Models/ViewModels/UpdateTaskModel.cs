using Project1.Models.ENums;
using TaskStatus = Project1.Models.ENums.TaskStatus;

namespace Project1.Models.ViewModels;

public class UpdateTaskModel
{
    public string Description { get; set; }
    public TaskPriority Priority { get; set; }
    public TaskStatus Status { get; set; }
    
    public UpdateTaskModel()
    {
        Description = "";
        Priority = TaskPriority.Medium;
        Status = TaskStatus.NotStarted;
    }
}