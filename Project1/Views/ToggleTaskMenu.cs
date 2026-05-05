using Project1.Models;
using Project1.Models.ViewModels;
using Project1.Services.Interfaces;
using Project1.Views.Mapping;
using TaskStatus = Project1.Models.ENums.TaskStatus;

namespace Project1.Views;

public class ToggleTaskMenu
{
    private readonly ChoiceMenu _menu;
    private readonly TaskDisplayMapper _displayMapper;
    private readonly ITaskService _taskService;
    
    public ToggleTaskMenu(TaskDisplayMapper mapper, ITaskService taskService)
    {
        _menu = new ChoiceMenu();
        _displayMapper = mapper;
        _taskService = taskService;
    }

    public void ToggleTask(Func<IMyCollection<TaskItem>> getTasksWithFilter, int currentUserId, Func<int, bool> canEdit)
    {
        while (true)
        {
            MenuOption<TaskItem>[] menuItems = BuildTaskSelectionMenuItems(getTasksWithFilter());
            int selectedIndex = DisplayTaskSelectionMenu(menuItems);
            TaskStatus? newStatus = null;
            
            if (menuItems[selectedIndex].IsAction)
                return ;

            TaskItem selectedTask = menuItems[selectedIndex].Value!;
            
            if (!canEdit(selectedTask.Id))
                ToggleBlocked(selectedTask);
            else
                newStatus = HandleTaskToggle();

            if (_taskService.IsBlocked(selectedTask.Id))
            {
                if (selectedTask.Status < newStatus)
                {
                    ToggleBlockedByDependency(selectedTask);
                    continue;
                }

                if (selectedTask.Status == TaskStatus.Done && newStatus != TaskStatus.NotStarted)
                {
                    ToggleBlockedCanOnlyBeNotStarted(selectedTask);
                }
                continue;
            }

            if (newStatus != null)
                _taskService.ToggleTask(selectedTask.Id, currentUserId, newStatus.GetValueOrDefault());
        }
    }
    
    private int DisplayTaskSelectionMenu(MenuOption<TaskItem>[] menuItems)
    {
        Console.Clear();

        int selectedIndex = _menu.GetChoice(menuItems, true, "=== Choose Task To Toggle ===\n\n");

        return selectedIndex;
    }
    
    private MenuOption<TaskItem>[] BuildTaskSelectionMenuItems(IMyCollection<TaskItem> tasks)
    {
        MenuOption<TaskItem>[] menuItems = new MenuOption<TaskItem>[tasks.Count + 1];
        
        IMyIterator<TaskItem> iterator = tasks.GetIterator();
        int p = 0;
        while (iterator.HasNext())
        {
            TaskItem task = iterator.Next();
            menuItems[p++] = new MenuOption<TaskItem>(task, _displayMapper.Map(task).ToString());
        }
        
        menuItems[^1] = new MenuOption<TaskItem>("Exit");

        return menuItems;
    }
    
    private TaskStatus? HandleTaskToggle()
    {
        Console.Clear();
        Console.WriteLine("=== Enter Status ===\n");

        string?[] statuses =
        [
            Enum.GetName(typeof(TaskStatus), TaskStatus.NotStarted)!,
            Enum.GetName(typeof(TaskStatus), TaskStatus.InProgress)!,
            Enum.GetName(typeof(TaskStatus), TaskStatus.Done)!,
            null,
            "Exit"
        ];
        int selectedIndex = _menu.GetChoice(statuses);
        
        if (selectedIndex >= 3)
            return null;
        
        return (TaskStatus)selectedIndex;
    }
    
    private void ToggleBlocked(TaskItem task)
    {
        Console.Clear();
        Console.CursorVisible = false;
        Console.WriteLine($"=== Toggle #{task.Id} {task.Description} ===\n");
        Console.WriteLine("You don't have permission to toggle this task status");
        Console.WriteLine("Press any key to continue");
        Console.ReadKey();
        Console.CursorVisible = true;
    }

    private void ToggleBlockedByDependency(TaskItem task)
    {
        Console.Clear();
        Console.CursorVisible = false;
        Console.WriteLine($"=== Toggle #{task.Id} {task.Description} ===\n");
        Console.WriteLine($"Task toggling is blocked by dependencies: {string.Join(", ", _taskService.GetBlockingTasksIds(task.Id))}");
        Console.WriteLine("Press any key to continue");
        Console.ReadKey();
        Console.CursorVisible = true;
    }
    
    private void ToggleBlockedCanOnlyBeNotStarted(TaskItem task)
    {
        Console.Clear();
        Console.CursorVisible = false;
        Console.WriteLine($"=== Toggle #{task.Id} {task.Description} ===\n");
        Console.WriteLine($"Task toggling is blocked by dependencies: {string.Join(", ", _taskService.GetBlockingTasksIds(task.Id))}");
        Console.WriteLine("This task can only be set as NotStarted");
        Console.WriteLine("Press any key to continue");
        Console.ReadKey();
        Console.CursorVisible = true;
    }
}