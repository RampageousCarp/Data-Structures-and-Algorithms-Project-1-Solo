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
    
    public static TaskTableView FromTask(TaskItem task) => new TaskTableView
    {
        Id = task.Id,
        Description = task.Description,
        Priority = task.Priority,
        Status = task.Status,
        CreatedAt = task.CreatedAt
    };

    public string ToTableString()
    {
        string id = $"[ID: {Id}]";
        id += new string(' ', 54 - id.Length) + "\n";
        
        string description = $"Desc: {Description}";
        description = description.Length >= 53
            ? description.Substring(0, 52) + "\n"
            : description + new string(' ', 54 - description.Length) + "\n";

        string priority = $"Prio: {Priority}";
        priority += new string(' ', 54 - priority.Length) + "\n";

        string created = $"Created: {CreatedAt:dd-MM-yyyy}";
        created += new string(' ', 54 - created.Length);

        string display = id + description + priority + created;

        return display;
    }
}