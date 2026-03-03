using Project1.Models.ENums;
using TaskStatus = Project1.Models.ENums.TaskStatus;

namespace Project1.Models.ViewModels;

public class CreateUpdateTaskModel
{
    public string Description { get; set; }
    public TaskPriority Priority { get; set; }
    public TaskStatus Status { get; set; }
    
    public CreateUpdateTaskModel()
    {
        Description = "";
        Priority = TaskPriority.Medium;
        Status = TaskStatus.NotStarted;
    }
}