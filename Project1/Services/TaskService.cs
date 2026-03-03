using Project1.Models;
using Project1.Models.ViewModels;
using Project1.Repositories.Interfaces;
using Project1.Services.Interfaces;
using TaskStatus = Project1.Models.ENums.TaskStatus;

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

    public TaskDisplay[] LoadTasksForDisplay()
    {
        _tasks.Sort((t1, t2) => t1.Id.CompareTo(t2.Id));
        IMyIterator<TaskItem> tasks = _tasks.GetIterator();
        tasks.Reset();

        TaskDisplay[] tasksForDisplay = new TaskDisplay[_tasks.Count];

        int pos = -1;
        while (tasks.HasNext())
        {
            TaskItem currTask = tasks.Next();
            tasksForDisplay[++pos] = new TaskDisplay{Id = currTask.Id, Description = currTask.Description, Priority = currTask.Priority, Status = currTask.Status};
        }

        return tasksForDisplay;
    }

    public void AddTask(CreateUpdateTaskModel createTaskData)
    {
        TaskItem newTask = new TaskItem 
        {
            Id = ++_lastId,
            Description = createTaskData.Description,
            Priority = createTaskData.Priority,
            Status = createTaskData.Status,
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
            _tasks.Dirty = true;
        }
    }

    public void UpdateTask(int id, CreateUpdateTaskModel updateTaskData)
    {
        TaskItem? task = _tasks.FindBy(id, (t, key) => t.Id == key);
        if (task is null)
            return;

        task.Description = updateTaskData.Description;
        task.Priority = updateTaskData.Priority;
        task.Status = updateTaskData.Status;
        
        _tasks.Dirty = true;
    }

    public void ToggleTask(int id, TaskStatus newStatus)
    {
        TaskItem? task = _tasks.FindBy(id, (t, key) => t.Id == key);
        if (task is null)
            return;
        
        task.Status = newStatus;
        
        _tasks.Dirty = true;
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
        int lastId = 0;
        while (items.HasNext())
        {
            int currId = items.Next().Id;
            if (lastId < currId)
                lastId = currId;
        }

        return lastId;
    }
}