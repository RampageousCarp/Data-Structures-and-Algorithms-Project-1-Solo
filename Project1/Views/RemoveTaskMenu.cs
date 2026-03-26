using Project1.Models;
using Project1.Models.ViewModels;
using Project1.Services.Interfaces;

namespace Project1.Views;

public class RemoveTaskMenu
{
    private ChoiceMenu _menu;
    
    public RemoveTaskMenu()
    {
        _menu = new ChoiceMenu();
    }
    

    public int RemoveTask(IMyCollection<TaskItem> tasks)
    {
        MenuOption<TaskItem>[] menuItems = BuildTaskSelectionMenuItems(tasks);
        
        while (true)
        {
            int selectedIndex = DisplayTaskSelectionMenu(menuItems);
            
            if (menuItems[selectedIndex].IsAction)
                return -1;

            if (ConfirmRemove(menuItems[selectedIndex].Value!.ConvertTo<TaskDisplay>()))
                return menuItems[selectedIndex].Value!.Id;
        }
    }
    
    private int DisplayTaskSelectionMenu(MenuOption<TaskItem>[] menuItems)
    {
        Console.Clear();

        int selectedIndex = _menu.GetChoice(menuItems, true, "=== Choose Task To Remove ===\n\n");

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
            menuItems[p++] = new MenuOption<TaskItem>(task, task.ConvertTo<TaskDisplay>().ToString());
        }
        
        menuItems[^1] = new MenuOption<TaskItem>("Exit");

        return menuItems;
    }

    private bool ConfirmRemove(TaskDisplay task)
    {
        Console.Clear();
        Console.WriteLine($"=== Remove #{task.Id} {task.Description} ===\n");
        
        return _menu.GetChoice(["Yes", "No"]) == 0;
    }
}