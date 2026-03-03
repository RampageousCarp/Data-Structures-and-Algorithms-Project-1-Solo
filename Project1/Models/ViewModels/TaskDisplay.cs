using Project1.Models.ENums;
using TaskStatus = Project1.Models.ENums.TaskStatus;

namespace Project1.Models.ViewModels;

public class TaskDisplay
{
    public int Id { get; set; }
    public string Description { get; set; } = null!;
    public TaskPriority Priority;
    public TaskStatus Status;

    public override string ToString()
    {
        string display = $"#{Id} {Description}\n";
        display += $"Priority: {Priority}\n";
        display += $"Status: {Status}\n";
        
        return display;
    }
}