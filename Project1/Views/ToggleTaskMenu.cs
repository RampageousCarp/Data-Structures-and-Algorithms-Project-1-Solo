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

    public (int id, TaskStatus status)? ToggleTask(IMyCollection<TaskItem> tasks)
    {
        MenuOption<TaskItem>[] menuItems = BuildTaskSelectionMenuItems(tasks);
        
        while (true)
        {
            int selectedIndex = DisplayTaskSelectionMenu(menuItems);
            
            if (menuItems[selectedIndex].IsAction)
                return null;

            TaskStatus? newStatus = HandleTaskToggle();
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
}