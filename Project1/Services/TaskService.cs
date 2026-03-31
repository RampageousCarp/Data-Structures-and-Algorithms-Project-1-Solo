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
    private readonly IUserService _userService;
    private int _lastId;
    
    public TaskService(IGenericRepository<TaskItem> repository, IMyCollection<TaskItem> collection, IMyCollectionFactory collectionFactory, IUserService userService)
    {
        _repository = repository;
        _tasks = collection;
        _collectionFactory = collectionFactory;
        _userService = userService;
        _lastId = LoadLastId(_tasks.GetIterator());
    }

    public IMyCollection<TaskItem> GetAllTasksWithFilter(TaskFilter? filter)
    {
        Func<TaskItem, bool> predicate = filter is null || filter.IsEmpty
            ? _ => true
            : BuildPredicate(filter);

        IMyCollection<TaskItem> filteredCollection = _tasks.Filter(predicate);

        Comparison<TaskItem> comparison = BuildComparison(filter);
        
        filteredCollection.Sort(comparison);

        return filteredCollection;
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

        return new GroupedTasks(todo, inProgress, done);

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
        TaskItem? task = _tasks.FindBy(id, (t, key) => t.Id.CompareTo(key));
        if (task is not null)
        {
            _tasks.Remove(task);
            _tasks.Dirty = true;
        }
    }

    public void UpdateTask(int id, UpdateTaskModel updateTaskData)
    {
        TaskItem? task = _tasks.FindBy(id, (t, key) => t.Id.CompareTo(key));
        if (task is null)
            return;

        task.Description = updateTaskData.Description;
        task.Priority = updateTaskData.Priority;
        task.Status = updateTaskData.Status;
        
        _tasks.Dirty = true;
    }

    public void ToggleTask(int id, TaskStatus newStatus)
    {
        TaskItem? task = _tasks.FindBy(id, (t, key) => t.Id.CompareTo(key));
        if (task is null)
            return;
        
        task.Status = newStatus;
        
        _tasks.Dirty = true;
    }

    public bool CanUserEdit(int taskId, int currentUserId)
    {
        TaskItem? task = _tasks.FindBy<int>(taskId, (t, key) => t.Id.CompareTo(key));
        if (task is null)
            return true;

        return task.AssignedTo == currentUserId;
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
        
        if (filter.Assignee != 0 && filter.Assignee != -1)
        {
            Func<TaskItem, bool> prev = predicate;
            predicate = task => prev(task) && task.AssignedTo == filter.Assignee;
        }
        
        if (filter.Assignee == -1)
        {
            Func<TaskItem, bool> prev = predicate;
            predicate = task => prev(task) && task.AssignedTo is null;
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
         
         if (filter.SortBy == SortingValue.Assignee)
             return (t1, t2) =>
             {
                 var user1 = _userService.GetUserById(t1.AssignedTo.GetValueOrDefault());
                 var user2 = _userService.GetUserById(t2.AssignedTo.GetValueOrDefault());

                 string name1 = user1?.Username ?? "";
                 string name2 = user2?.Username ?? "";

                 return string.Compare(name1, name2, StringComparison.Ordinal) * order;
             };

         return (t1, t2) => t1.Id.CompareTo(t2.Id);
     }
    
}