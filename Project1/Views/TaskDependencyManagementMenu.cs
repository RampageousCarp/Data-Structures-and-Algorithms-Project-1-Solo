using Project1.Models;
using Project1.Models.ViewModels;
using Project1.Services.Interfaces;
using Project1.Views.Mapping;
using TaskStatus = Project1.Models.ENums.TaskStatus;

namespace Project1.Views;

public class TaskDependencyManagementMenu
{
    private readonly ChoiceMenu _menu;
    private readonly TaskDisplayMapper _displayMapper;
    private readonly ITaskService _taskService;
    
    public TaskDependencyManagementMenu(TaskDisplayMapper mapper, ITaskService taskService)
    {
        _menu = new ChoiceMenu();
        _displayMapper = mapper;
        _taskService = taskService;
    }
    
    public void ManageDependencies(Func<IMyCollection<TaskItem>> getTasksWithFilter, Func<int, bool> canEdit)
    {
        while (true)
        {
            MenuOption<TaskItem>[] menuItems = BuildTaskSelectionMenuItems(getTasksWithFilter());
            int selectedIndex = DisplayTaskSelectionMenu(menuItems);
            
            if (menuItems[selectedIndex].IsAction)
                return;
            
            if (!canEdit(menuItems[selectedIndex].Value!.Id))
                ManagementBlocked(menuItems[selectedIndex].Value!);
            else
                DisplayManagementActions(menuItems[selectedIndex].Value!);
        }
    }
    
    private int DisplayTaskSelectionMenu(MenuOption<TaskItem>[] menuItems, string title = "=== Choose Task To Manage Dependencies ===\n\n")
    {
        Console.Clear();

        int selectedIndex = _menu.GetChoice(menuItems, true, title);

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
    
    private void ManagementBlocked(TaskItem task)
    {
        Console.Clear();
        Console.CursorVisible = false;
        Console.WriteLine($"=== Manage Dependencies #{task.Id} {task.Description} ===\n");
        Console.WriteLine("You don't have permission to manage dependencies of this task");
        Console.WriteLine("Press any key to continue");
        Console.ReadKey();
        Console.CursorVisible = true;
    }

    private void DisplayManagementActions(TaskItem task)
    {
        Console.Clear();

        string?[] menuItems = ["Add dependency", "Remove dependency", "Remove all dependencies", null, "Exit"];

        while (true)
        {
            int selectedIndex = _menu.GetChoice(menuItems, true, "=== Choose Management Action ===\n\n");
            
            switch (selectedIndex)
            {
                case 0:
                    AddDependency(task);
                    continue;
                case 1:
                    RemoveDependency(task);
                    continue;
                case 2:
                    if (ConfirmAllDependenciesRemove(task))
                        _taskService.RemoveAllDependencies(task.Id);
                    continue;
                default:
                    return;
            }
        }
    }

    private void RemoveDependency(TaskItem task)
    {
        MenuOption<TaskItem>[] menuItems = BuildTaskSelectionMenuItems(_taskService.GetAllDependencyTasks(task.Id));
        int selectedIndex = DisplayTaskSelectionMenu(menuItems, "=== Choose Dependency To Remove ===\n\n");
        
        if (menuItems[selectedIndex].IsAction)
            return;
        
        if (ConfirmDependencyRemove(task, menuItems[selectedIndex].Value!))
            _taskService.RemoveDependency(task.Id, menuItems[selectedIndex].Value!.Id);
    }

    private void AddDependency(TaskItem task)
    {
        while (true)
        {
            MenuOption<TaskItem>[] menuItems = BuildTaskSelectionMenuItems(_taskService.GetAllTasksWithFilter(null));
            int selectedIndex = DisplayTaskSelectionMenu(menuItems, "=== Choose Task To Add Dependency ===\n\n");

            if (menuItems[selectedIndex].IsAction)
                return;

            TaskItem chosenTask = menuItems[selectedIndex].Value!;
            if (_taskService.WouldCreateCycle(task.Id, chosenTask.Id))
            {
                BlockDependencyDueCircularReference(task, chosenTask);
                continue;
            }

            if (_taskService.AlreadyInDependsOn(task.Id, chosenTask.Id))
            {
                BlockDependencyAlreadyDepends(task, chosenTask);
                continue;
            }

            if (task.Status == TaskStatus.InProgress && !chosenTask.Completed &&
                ConfirmTaskMoveToTodo(task, chosenTask))
            {
                _taskService.AddDependency(task.Id, chosenTask.Id);
                _taskService.SetTaskInToDo(task.Id);
                return;
            }
            
            _taskService.AddDependency(task.Id, chosenTask.Id);
            return;
        }
    }

    private bool ConfirmAllDependenciesRemove(TaskItem task)
    {
        Console.Clear();
        Console.WriteLine($"=== Remove All Dependencies For #{task.Id} {task.Description} ===\n");
        
        return _menu.GetChoice(["Yes", "No"]) == 0;
    }
    
    private bool ConfirmDependencyRemove(TaskItem task, TaskItem dependency)
    {
        Console.Clear();
        Console.WriteLine($"=== Remove #{dependency.Id} {dependency.Description} As Dependency For #{task.Id} {task.Description} ===\n");
        
        return _menu.GetChoice(["Yes", "No"]) == 0;
    }

    private void BlockDependencyDueCircularReference(TaskItem task, TaskItem dependency)
    {
        Console.Clear();
        Console.CursorVisible = false;
        Console.WriteLine($"=== Adding Dependency #{dependency.Id} {dependency.Description} To Task #{task.Id} {task.Description} ===\n");
        Console.WriteLine($"#{dependency.Id} {dependency.Description} cannot be added as dependency to the #{task.Id} {task.Description} due to circular reference");
        Console.WriteLine("Press any key to continue");
        Console.ReadKey();
        Console.CursorVisible = true;
    }

    private bool ConfirmTaskMoveToTodo(TaskItem task, TaskItem dependency)
    {
        Console.Clear();
        Console.WriteLine($"=== Task #{dependency.Id} {dependency.Description} Is Not Completed. Task #{task.Id} {task.Description} Will Be Replaced To TODO ===\nContinue?\n");
        
        return _menu.GetChoice(["Yes", "No"]) == 0;
    }

    private void BlockDependencyAlreadyDepends(TaskItem task, TaskItem dependency)
    {
        Console.Clear();
        Console.CursorVisible = false;
        Console.WriteLine($"=== Adding Dependency #{dependency.Id} {dependency.Description} To Task #{task.Id} {task.Description} ===\n");
        Console.WriteLine($"#{task.Id} {task.Description} already depends on #{dependency.Id} {dependency.Description}");
        Console.WriteLine("Press any key to continue");
        Console.ReadKey();
        Console.CursorVisible = true;
    }
        
    
}