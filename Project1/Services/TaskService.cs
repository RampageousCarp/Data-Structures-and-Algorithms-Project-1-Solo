using Project1.Models;
using Project1.Models.ENums;
using Project1.Models.ViewModels;
using Project1.Repositories.Interfaces;
using Project1.Services.Interfaces;
using TaskStatus = Project1.Models.ENums.TaskStatus;

namespace Project1.Services;
class TaskService : ITaskService
{
    private readonly IGenericRepository<TaskItem> _repository;
    private readonly IMyCollection<TaskItem> _tasks;
    private readonly IMyCollectionFactory _collectionFactory;
    private int _lastId;
    
    public TaskService(IGenericRepository<TaskItem> repository, IMyCollection<TaskItem> collection, IMyCollectionFactory collectionFactory)
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

    public TaskItem[] GetAllTasksSorted(TaskFilter? filter)
    {
        Func<TaskItem, bool> predicate = filter is null || filter.IsEmpty
            ? _ => true
            : BuildPredicate(filter);

        IMyCollection<TaskItem> filteredCollection = _tasks.Filter(predicate);

        Comparison<TaskItem> comparison = BuildComparison(filter);
        
        filteredCollection.Sort(comparison);
        
        IMyIterator<TaskItem> tasks = filteredCollection.GetIterator();
        tasks.Reset();

        TaskItem[] tasksToReturn = new TaskItem[filteredCollection.Count];

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
        
        Comparison<TaskItem> comparison = BuildComparison(filter);

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
        
        todo.Sort(comparison);
        inProgress.Sort(comparison);
        done.Sort(comparison);
        
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
            _repository.SaveItems(_tasks.GetIterator(), _tasks.Count);
        
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
            predicate = task => prev(task) && task.Status == filter.Status;
        }
        
        if (filter.Priority is not null)
        {
            Func<TaskItem, bool> prev = predicate;
            predicate = task => prev(task) && task.Priority == filter.Priority;
        }
        
        if (filter.DueToFrom is not null)
        {
            Func<TaskItem, bool> prev = predicate;
            predicate = task => prev(task) && task.DueTo >= filter.DueToFrom;
        }
        
        if (filter.DueToTo is not null)
        {
            Func<TaskItem, bool> prev = predicate;
            predicate = task => prev(task) && task.DueTo <= filter.DueToTo;
        }
        
        if (filter.CreatedAtFrom is not null)
        {
            Func<TaskItem, bool> prev = predicate;
            predicate = task => prev(task) && DateOnly.FromDateTime(task.CreatedAt) >= filter.CreatedAtFrom;
        }
        
        if (filter.CreatedAtTo is not null)
        {
            Func<TaskItem, bool> prev = predicate;
            predicate = task => prev(task) && DateOnly.FromDateTime(task.CreatedAt) <= filter.CreatedAtTo;
        }
        
        if (!string.IsNullOrWhiteSpace(filter.Keyword))
        {
            Func<TaskItem, bool> prev = predicate;
            predicate = task => prev(task) && task.Description.Contains(filter.Keyword, StringComparison.OrdinalIgnoreCase);
        }
        
        return predicate;
     }

     private Comparison<TaskItem> BuildComparison(TaskFilter? filter)
     {

         if (filter is null || !filter.ApplySort)
             return (t1, t2) => t1.Id.CompareTo(t2.Id);
         
         int order = filter.SortOrder == SortOrder.Ascending ? 1 : -1;

         if (filter.SortBy == SortingValue.ID)
             return (t1, t2) => t1.Id.CompareTo(t2.Id) * order;
         
         if (filter.SortBy == SortingValue.Description)
             return (t1, t2) => String.Compare(t1.Description, t2.Description, StringComparison.Ordinal) * order;
         
         if (filter.SortBy == SortingValue.Priority)
             return (t1, t2) => t1.Priority.CompareTo(t2.Priority) * order;
         
         if (filter.SortBy == SortingValue.CreatedAt)
             return (t1, t2) => t1.CreatedAt.CompareTo(t2.CreatedAt) * order;
         
         if (filter.SortBy == SortingValue.DueTo)
             return (t1, t2) => t1.DueTo.CompareTo(t2.DueTo) * order;

         return (t1, t2) => t1.Id.CompareTo(t2.Id);
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