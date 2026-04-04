using Project1.Models;
using Project1.Models.ViewModels;
using Project1.Services.Interfaces;
using Project1.Views.Mapping;

namespace Project1.Views;

public class RemoveTaskMenu
{
    private ChoiceMenu _menu;
    private TaskDisplayMapper _displayMapper;
    
    public RemoveTaskMenu(TaskDisplayMapper mapper)
    {
        _menu = new ChoiceMenu();
        _displayMapper = mapper;
    }
    

    public int RemoveTask(IMyCollection<TaskItem> tasks, Func<int, bool> canEdit)
    {
        MenuOption<TaskItem>[] menuItems = BuildTaskSelectionMenuItems(tasks);
        
        while (true)
        {
            int selectedIndex = DisplayTaskSelectionMenu(menuItems);
            int result = -1;
            
            if (menuItems[selectedIndex].IsAction)
                return -1;
            
            if (canEdit(menuItems[selectedIndex].Value!.Id))
                result = menuItems[selectedIndex].Value!.Id;
            else
                RemoveBlocked(menuItems[selectedIndex].Value!);

            if (result != -1 && ConfirmRemove(_displayMapper.Map(menuItems[selectedIndex].Value!)))
                return result;
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
            menuItems[p++] = new MenuOption<TaskItem>(task, _displayMapper.Map(task).ToString());
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

    private void RemoveBlocked(TaskItem task)
    {
        Console.Clear();
        Console.CursorVisible = false;
        Console.WriteLine($"=== Remove #{task.Id} {task.Description} ===\n");
        Console.WriteLine("You don't have permission to remove this task");
        Console.WriteLine("Press any key to continue");
        Console.ReadKey();
        Console.CursorVisible = true;
    }
}