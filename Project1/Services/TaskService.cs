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

    private const int DIRTY_LIMIT = 10;
    
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

        Comparison<TaskItem>? comparison = BuildComparison(filter);
        
        filteredCollection.Sort(comparison);

        return filteredCollection;
    }

    public GroupedTasks GetGroupedTasks(TaskFilter? filter)
    {
        Func<TaskItem, bool> predicate = filter is null || filter.IsEmpty
            ? _ => true
            : BuildPredicate(filter);
        
        Comparison<TaskItem>? comparison = BuildComparison(filter);

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
            CreatedAt = DateTime.UtcNow,
            DueTo = createTaskData.DueTo,
            AssignedTo = createTaskData.AssignedTo
        };
        
        _tasks.Add(newTask);
        _tasks.IncreaseDirty();
        AutoSave();
    }

    public bool RemoveTask(int id, int currentUserId)
    {
        TaskItem? task = GetTaskById(id);
        if (task is null || (task.AssignedTo is not null && task.AssignedTo != currentUserId))
            return false;
        
        _tasks.Remove(task);
        ClearDependencyReferences(id);
        
        _tasks.IncreaseDirty();
        AutoSave();

        return true;

    }

    public bool UpdateTask(int id, int currentUserId, UpdateTaskModel updateTaskData)
    {
        TaskItem? task = GetTaskById(id);
        if (task is null || (task.AssignedTo is not null && task.AssignedTo != currentUserId))
            return false;

        task.Description = updateTaskData.Description;
        task.Priority = updateTaskData.Priority;
        task.Status = updateTaskData.Status;
        task.DueTo = updateTaskData.DueTo;
        task.AssignedTo = updateTaskData.AssignedTo;
        
        _tasks.IncreaseDirty();
        AutoSave();

        return true;
    }

    public bool ToggleTask(int id, int currentUserId, TaskStatus newStatus)
    {
        TaskItem? task = GetTaskById(id);
        if (task is null || (task.AssignedTo is not null && task.AssignedTo != currentUserId))
            return false;
        
        task.Status = newStatus;
        
        _tasks.IncreaseDirty();
        AutoSave();
        
        return true;
    }

    public bool AssignTask(int id, int currentUserId, int? newAssignee)
    {
        TaskItem? task = GetTaskById(id);
        if (task is null || (task.AssignedTo is not null && task.AssignedTo != currentUserId))
            return false;

        task.AssignedTo = newAssignee;
        
        _tasks.IncreaseDirty();
        AutoSave();
        
        return true;
    }

    public void UnassignUser(int userId)
    {
        IMyIterator<TaskItem> iterator = _tasks.GetIterator();
        iterator.Reset();
        while (iterator.HasNext())
        {
            TaskItem task = iterator.Next();
            if (task.AssignedTo == userId)
                task.AssignedTo = null;
        }

        _tasks.IncreaseDirty();
        AutoSave();
    }

    public bool CanUserEdit(int taskId, int currentUserId)
    {
        TaskItem? task = GetTaskById(taskId);
        if (task is null)
            return true;
        
        return (task.AssignedTo == currentUserId || task.AssignedTo is null) ;
    }

    public void SaveTasks()
    {
        if (_tasks.Dirty)
            _repository.SaveItems(_tasks.GetIterator(), _tasks.Count);
        
        _tasks.ResetDirty();
    }

    public bool IsBlocked(int taskId)
    {
        TaskItem? task = GetTaskById(taskId);
        if (task is null)
            return false;

        for (int i = 0; i < task.DependsOn.Length; i++)
        {
            TaskItem? taskDependency = _tasks.FindBy(task.DependsOn[i], (t, key) => t.Id.CompareTo(key));
            if (taskDependency != null && !taskDependency.Completed)
                return true;
        }

        return false;
    }

    public int[] GetBlockingTasksIds(int taskId)
    {
        TaskItem? task = GetTaskById(taskId);
        if (task is null || task.DependsOn == null || task.DependsOn.Length == 0)
            return Array.Empty<int>();

        int[] blockingTasks = new int[task.DependsOn.Length];

        int pos = 0;
        for (int i = 0; i < task.DependsOn.Length; i++)
        {
            TaskItem? taskDependency = _tasks.FindBy(task.DependsOn[i], (t, key) => t.Id.CompareTo(key));
            if (taskDependency != null && !taskDependency.Completed)
                blockingTasks[pos++] = taskDependency.Id;
        }

        if (pos == 0)
            return Array.Empty<int>();
        
        Array.Resize(ref blockingTasks, pos);
        return blockingTasks;
    }

    public IMyCollection<TaskItem> GetAllDependencyTasks(int taskId)
    {
        
        TaskItem? task = GetTaskById(taskId);
        if (task is null || task.DependsOn == null || task.DependsOn.Length == 0)
            return _collectionFactory.Create<TaskItem>();

        Func<TaskItem, bool> predicate = taskItem =>
        {
            for (int i = 0; i < task.DependsOn.Length; i++)
            {
                if (task.DependsOn[i] == taskItem.Id)
                {
                    return true;
                }
            }
            return false;
        };

        return _tasks.Filter(predicate);
    }

    public void RemoveDependency(int taskId, int dependencyTaskId)
    {
        TaskItem? task = GetTaskById(taskId);

        if (task is null)
            return;

        task.DependsOn = ArrayRemove(task.DependsOn, dependencyTaskId);
        
        _tasks.IncreaseDirty();
        AutoSave();
    }

    public void RemoveAllDependencies(int taskId)
    {
        TaskItem? task = GetTaskById(taskId);

        if (task is null)
            return;

        task.DependsOn = Array.Empty<int>();
        
        _tasks.IncreaseDirty();
        AutoSave();

    }

    public bool WouldCreateCycle(int taskId, int dependencyId)
    {
        if (taskId == dependencyId)
            return true;
        
        int maxSize = _lastId + 1;

        int[] toVisit = new int[maxSize];
        bool[] visited = new bool[maxSize];

        int head = 0, tail = 0;

        toVisit[tail++] = dependencyId;
        visited[dependencyId] = true;

        while (head < tail)
        {
            int current = toVisit[head++];

            if (current == taskId)
                return true;

            TaskItem? task = _tasks.FindBy(current, (item, key) => item.Id.CompareTo(key));

            if (task == null || task.DependsOn == null || task.DependsOn.Length <= 0)
                continue;

            for (int i = 0; i < task.DependsOn.Length; i++)
            {
                int nextId = task.DependsOn[i];

                if (nextId >= 0 && nextId < maxSize && !visited[nextId])
                {
                    visited[nextId] = true;
                    if (tail < toVisit.Length)
                        toVisit[tail++] = nextId;
                }
            }
        }
        
        return false;
    }

    private void AutoSave()
        {
            if (_tasks.GetDirtyCount() >= DIRTY_LIMIT)
                SaveTasks();
        
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

        private Comparison<TaskItem>? BuildComparison(TaskFilter? filter)
        {

            if (filter is null || !filter.ApplySort)
                return null;
         
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

        private void ClearDependencyReferences(int taskId)
        {
            IMyIterator<TaskItem> iterator = _tasks.GetIterator();
            iterator.Reset();
            while (iterator.HasNext())
            {
                TaskItem task = iterator.Next();
                task.DependsOn = ArrayRemove(task.DependsOn, taskId);
             
                _tasks.IncreaseDirty();
            }
        }

        private int[] ArrayRemove(int[] arr, int value)
        {
            int count = 0;
            for (int i = 0; i < arr.Length; i ++)
                if (arr[i] == value)
                    count++;

            if (count == 0)
                return arr;

            int[] result = new int[arr.Length - count];
            int pos = 0;
            for(int i = 0; i < arr.Length; i ++)
                if (arr[i] != value)
                    result[pos++] = arr[i];

            return result;
        }

        private TaskItem? GetTaskById(int taskId)
        {
            return _tasks.FindBy(taskId, (t, key) => t.Id.CompareTo(key));
        }

}