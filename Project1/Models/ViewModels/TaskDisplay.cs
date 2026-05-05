using Project1.Models.ENums;
using Project1.Models.Interfaces;
using TaskStatus = Project1.Models.ENums.TaskStatus;

namespace Project1.Models.ViewModels;

public class TaskDisplay
{
    public int Id { get; set; }
    public string Description { get; set; } = null!;
    public TaskPriority Priority { get; set; }
    public TaskStatus Status { get; set; }
    public DateOnly DueTo { get; set; }
    public string AssigneeName { get; set; } = "Unassigned";
    public int[] DependsOn { get; set; } = Array.Empty<int>();

    public override string ToString()
    {
        string display = $"#{Id} {Description}\n";
        display += $"Priority: {Priority}\n";
        display += $"Status: {Status}\n";
        display += $"Due To: {DueTo:dd-MM-yyyy}\n";
        display += $"Assigned to: {AssigneeName}\n";
        display += $"Depends On: {(DependsOn.Length > 0 ? string.Join(", ", DependsOn) : "None")}\n";
        
        return display;
    }
}