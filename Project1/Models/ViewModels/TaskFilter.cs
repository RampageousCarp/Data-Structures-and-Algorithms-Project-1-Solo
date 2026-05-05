using Project1.Models.ENums;
using TaskStatus = Project1.Models.ENums.TaskStatus;

namespace Project1.Models.ViewModels;

public class TaskFilter
{
    public TaskStatus? Status { get; set; }
    public TaskPriority? Priority { get; set; }
    public DateOnly? DueToFrom { get; set; }
    public DateOnly? DueToTo { get; set; }
    public DateOnly? CreatedAtFrom { get; set; }
    public DateOnly? CreatedAtTo { get; set; }
    public string? Keyword { get; set; }
    public int Assignee { get; set; } = 0;

    public string? AssigneeUsername
    {
        get
        {
            if (Assignee == 0)
                return "Unassigned";
            
            return _assigneeUsername;
        }
        set => _assigneeUsername = value;
    }

    private string? _assigneeUsername { get; set; }

    public SortingValue? SortBy { get; set; }
    public SortOrder SortOrder { get; set; } = SortOrder.Ascending;

    public bool IsEmpty =>
        Status is null &&
        Priority is null &&
        DueToFrom is null &&
        DueToTo is null &&
        CreatedAtFrom is null &&
        CreatedAtTo is null &&
        Keyword is null &&
        Assignee == 0;

    public bool ApplySort =>
        SortBy is not null;

    public void ResetFilters()
    {
        Status = null;
        Priority = null;
        DueToFrom = null;
        DueToTo = null;
        CreatedAtFrom = null;
        CreatedAtTo = null;
        Keyword = null;
        Assignee = 0;
        SortBy = null;
        SortOrder = SortOrder.Ascending;
    }
}