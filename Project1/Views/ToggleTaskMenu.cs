using Project1.Models;
using Project1.Models.ViewModels;
using Project1.Services.Interfaces;
using Project1.Views.Mapping;
using TaskStatus = Project1.Models.ENums.TaskStatus;

namespace Project1.Views;

public class ToggleTaskMenu
{
    private readonly ChoiceMenu _menu;
    private TaskDisplayMapper _displayMapper;
    
    public ToggleTaskMenu(TaskDisplayMapper mapper)
    {
        _menu = new ChoiceMenu();
        _displayMapper = mapper;
    }

    public (int id, TaskStatus status)? ToggleTask(IMyCollection<TaskItem> tasks, Func<int, bool> canEdit)
    {
        MenuOption<TaskItem>[] menuItems = BuildTaskSelectionMenuItems(tasks);
        
        while (true)
        {
            (int id, TaskStatus status)? result = null;
            TaskStatus? newStatus = null;
            int selectedIndex = DisplayTaskSelectionMenu(menuItems);
            
            if (menuItems[selectedIndex].IsAction)
                return null;
            
            if (canEdit(menuItems[selectedIndex].Value.Id))
                newStatus = HandleTaskToggle();
            else
                ToggleBlocked(menuItems[selectedIndex].Value!);
            
            if (newStatus is not null)
                return ((int id, TaskStatus status)?)(menuItems[selectedIndex].Value!.Id, newStatus);
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
}