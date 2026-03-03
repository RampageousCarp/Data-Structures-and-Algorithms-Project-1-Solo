using Project1.Models;
using Project1.Models.ViewModels;
using Project1.Repositories.Interfaces;
using Project1.Services.Interfaces;

namespace Project1.Services;
class TaskService : ITaskService
{
    private readonly ITaskRepository _repository;
    private readonly IMyCollection<TaskItem> _tasks;
    private int _lastId = 1;
    
    public TaskService(ITaskRepository repository, IMyCollection<TaskItem> collection)
    {
        _repository = repository;
        _tasks = collection;
        _lastId = LoadLastId(_tasks.GetIterator());
    }
    
    
    // public IEnumerable<TaskItem> GetAllTasks() => _tasks;
    
    // public void RemoveTask(int id)
    // {
    //     var task = _tasks.Find(t => t.Id == id);
    //     if (task != null)
    //     {
    //         _tasks.Remove(task);
    //         _repository.SaveTasks(_tasks);
    //     }
    // }
    // public void ToggleTaskCompletion(int id)
    // {
    //     var task = _tasks.Find(t => t.Id == id);
    //     if (task != null)
    //     {
    //         task.Completed = !task.Completed;
    //         _repository.SaveTasks(_tasks);
    //     }
    // }
    public IEnumerable<TaskItem> GetAllTasks()
    {
        throw new NotImplementedException();
    }

    public TaskSummary[] LoadTasksForDisplay()
    {
        IMyIterator<TaskItem> tasks = _tasks.GetIterator();
        tasks.Reset();

        TaskSummary[] tasksForDisplay = new TaskSummary[_tasks.Count];

        int pos = -1;
        while (tasks.HasNext())
        {
            TaskItem currTask = tasks.Next();
            tasksForDisplay[++pos] = new TaskSummary{Id = currTask.Id, Description = currTask.Description};
        }

        return tasksForDisplay;
    }

    public void AddTask(CreateTaskInput taskData)
    {
        TaskItem newTask = new TaskItem 
        {
            Id = ++_lastId,
            Description = taskData.Description,
            Priority = taskData.Priority,
            Status = taskData.Status,
            Completed = false,
            CreatedAt = DateTime.UtcNow
        };
        
        _tasks.Add(newTask);
        _tasks.Dirty = true;
    }

    public void RemoveTask(int id)
    {
        TaskItem? task = _tasks.FindBy(id, (t, key) => t.Id == key);
        if (task is not null)
        {
            _tasks.Remove(task);
            _tasks.Dirty
        }
    }

    public void ToggleTaskCompletion(int id)
    {
        throw new NotImplementedException();
    }

    public void SaveTasks()
    {
        if (_tasks.Dirty)
            _repository.SaveTasks(_tasks.GetIterator(), _tasks.Count);
        
        _tasks.Dirty = false;
    }

    private int LoadLastId(IMyIterator<TaskItem> items)
    {
        items.Reset();
        int lastId = (int)items.Next().Id;
        while (items.HasNext())
        {
            int currId = (int)items.Next().Id;
            if (lastId < currId)
                lastId = currId;
        }

        return lastId;
    }
}