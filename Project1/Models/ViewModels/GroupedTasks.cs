using Project1.Services.Interfaces;

namespace Project1.Models.ViewModels;

public class GroupedTasks
{
    public GroupedTasks(IMyCollection<TaskItem> todo, IMyCollection<TaskItem> inProgress, IMyCollection<TaskItem> done)
    {
        Todo = todo;
        InProgress = inProgress;
        Done = done;
    }

    public IMyCollection<TaskItem> Todo { get; set; }
    public IMyCollection<TaskItem> InProgress { get; set; }
    public IMyCollection<TaskItem> Done { get; set; }
}