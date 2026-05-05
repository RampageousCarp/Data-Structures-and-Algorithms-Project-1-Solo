using Project1.Models;
using Project1.Models.ViewModels;
using Project1.Services.Interfaces;
using Project1.Views.Mapping;

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
        MenuOption<TaskItem>[] menuItems;

        while (true)
        {
            menuItems = BuildTaskSelectionMenuItems(getTasksWithFilter());
            int selectedIndex = DisplayTaskSelectionMenu(menuItems);
            
            if (menuItems[selectedIndex].IsAction)
                return;
            
            if (!canEdit(menuItems[selectedIndex].Value!.Id))
                ManagementBlocked(menuItems[selectedIndex].Value!);
            else
                DisplayManagementActions(menuItems[selectedIndex].Value!);
        }
    }
    
    private int DisplayTaskSelectionMenu(MenuOption<TaskItem>[] menuItems)
    {
        Console.Clear();

        int selectedIndex = _menu.GetChoice(menuItems, true, "=== Choose Task To Manage Dependencies ===\n\n");

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
                    break;
                case 1:
                    break;
                case 2:
                    if (ConfirmAllDependenciesRemove(task))
                        _taskService.RemoveAllDependencies(task.Id);
                    continue;
                default:
                    return;
            }
        }
    }

    private bool ConfirmAllDependenciesRemove(TaskItem task)
    {
        Console.Clear();
        Console.WriteLine($"=== Remove All Dependencies For #{task.Id} {task.Description} ===\n");
        
        return _menu.GetChoice(["Yes", "No"]) == 0;
    }
    
    

}