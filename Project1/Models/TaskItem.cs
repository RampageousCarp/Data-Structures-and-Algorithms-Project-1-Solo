using System.Text.Json.Serialization;
using Project1.Models.ENums;
using TaskStatus = Project1.Models.ENums.TaskStatus;

namespace Project1.Models;

public class TaskItem
{
    public long Id { get; set; }
    
    public required string Description { get; set; }

    [JsonPropertyName("Priority")]
    private string _priority { get; set; } = null!;

    [JsonIgnore]
    public TaskPriority Priority
    {
        get => Enum.TryParse<TaskPriority>(_priority, out var priority) ? priority : TaskPriority.Medium;
        set => _priority = value.ToString();
    }

    [JsonPropertyName("Status")]
    private string _status = TaskStatus.NotStarted.ToString();

    [JsonIgnore]
    public TaskStatus Status
    {
        get => Enum.TryParse<TaskStatus>(_status, out var status) ? status : TaskStatus.NotStarted;
        set => _status = value.ToString();
    }

    public bool Completed { get; set; } = false;
    
    public DateTime CreatedAt { get; set; }
}