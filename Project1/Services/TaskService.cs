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
    private readonly IMyCollectionFactory _collectionFactory;
    private int _lastId = 1;
    
    public TaskService(ITaskRepository repository, IMyCollection<TaskItem> collection, IMyCollectionFactory collectionFactory)
    {
        _repository = repository;
        _tasks = collection;
        _collectionFactory = collectionFactory;
        _lastId = LoadLastId(_tasks.GetIterator());
    }
    
    public IEnumerable<TaskItem> GetAllTasks()
    {
        throw new NotImplementedException();
    }

    public TaskItem[] GetAllTasksSorted()
    {
        _tasks.Sort((t1, t2) => t1.Id.CompareTo(t2.Id));
        IMyIterator<TaskItem> tasks = _tasks.GetIterator();
        tasks.Reset();

        TaskItem[] tasksToReturn = new TaskItem[_tasks.Count];

        int pos = -1;
        while (tasks.HasNext())
        
            tasksToReturn[++pos] = tasks.Next();

        return tasksToReturn;
    }

    public TaskItem GetTasks(TaskFilter? filter = null)
    {
        throw new NotImplementedException();
    }

    public GroupedTasks GetGroupedTasks(TaskFilter? filter)
    {
        Func<TaskItem, bool> predicate = filter is null || filter.IsEmpty
            ? _ => true
            : BuildPredicate(filter);

        var (todo, inProgress, done) = _tasks.Reduce(
            (
                Todo: _collectionFactory.Create<TaskItem>(),
                InProgress: _collectionFactory.Create<TaskItem>(),
                Done: _collectionFactory.Create<TaskItem>()
            ),
            (acc,
                task) =>
            {
                if (!predicate(task))
                    return acc;

                switch (task.Status)
                {
                    case TaskStatus.NotStarted:
                        acc.Todo.Add(task);
                        break;
                    case TaskStatus.InProgress:
                        acc.InProgress.Add(task);
                        break;
                    case TaskStatus.Done:
                        acc.Done.Add(task);
                        break;
                }

                return acc;
            }
        );
        return new GroupedTasks
        {
            Todo = MapToTableView(todo),
            InProgress = MapToTableView(inProgress),
            Done = MapToTableView(done)
        };
    }

    public void AddTask(CreateTaskModel createTaskData)
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

    public void UpdateTask(int id, UpdateTaskModel updateTaskData)
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
    
    private Func<TaskItem, bool> BuildPredicate(TaskFilter filter)
    {
        Func<TaskItem, bool> predicate = _ => true;

        if (filter.Status is not null)
        {
            Func<TaskItem, bool> prev = predicate;
            TaskStatus status = filter.Status.Value;
            predicate = task => prev(task) && task.Status == status;
        }

        return predicate;
    }

    private TaskTableView[] MapToTableView(IMyCollection<TaskItem> tasks)
    {
        TaskTableView[] taskTableViews = new TaskTableView[tasks.Count];
        
        IMyIterator<TaskItem> iterator = tasks.GetIterator();
        iterator.Reset();

        int pos = -1;
        while (iterator.HasNext())
            taskTableViews[++pos] = TaskTableView.FromTask(iterator.Next());

        return taskTableViews;
    }
}