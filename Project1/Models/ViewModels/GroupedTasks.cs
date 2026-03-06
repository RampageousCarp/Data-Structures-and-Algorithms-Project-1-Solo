namespace Project1.Models.ViewModels;

public class GroupedTasks
{
    public TaskTableView[] Todo { get; set; }
    public TaskTableView[] InProgress { get; set; }
    public TaskTableView[] Done { get; set; }
}