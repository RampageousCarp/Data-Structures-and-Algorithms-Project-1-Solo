using Project1.Models.ViewModels;
using TaskStatus = Project1.Models.ENums.TaskStatus;

namespace Project1.Views;

public class ToggleTaskMenu
{
    private readonly ChoiceMenu<string> _menu;

    public ToggleTaskMenu()
    {
        _menu = new ChoiceMenu<string>();
    }

    public (int id, TaskStatus status)? ToggleTask(TaskDisplay[] tasks)
    {
        while (true)
        {
            int selectedIndex = DisplayTaskSelectionMenu(tasks);
            
            if (selectedIndex == -1)
                return null;

            TaskStatus? newStatus = HandleTaskToggle();
            if (newStatus is not null)
                return ((int id, TaskStatus status)?)(tasks[selectedIndex].Id, newStatus);
        }
    }
    
    private int DisplayTaskSelectionMenu(TaskDisplay[] tasks)
    {
        Console.Clear();

        string[] menuItems = BuildTaskSelectionMenuItems(tasks);
        int selectedIndex = _menu.GetChoice(menuItems, true, "=== Choose Task To Toggle ===\n\n");

        return selectedIndex == menuItems.Length - 1 ? -1 : selectedIndex;
    }
    
    private string[] BuildTaskSelectionMenuItems(TaskDisplay[] tasks)
    {
        string[] menuItems = new string[tasks.Length + 1];
        
        for (int i = 0; i < tasks.Length; i++)
            menuItems[i] = tasks[i].ToMenuString();

        menuItems[^1] = "Exit";

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