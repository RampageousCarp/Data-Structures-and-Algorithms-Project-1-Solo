using TaskStatus = Project1.Models.ENums.TaskStatus;

namespace Project1.Models.ViewModels;

public class TaskFilter
{
    public TaskStatus? Status { get; set; }

    public bool IsEmpty => Status is null;
}