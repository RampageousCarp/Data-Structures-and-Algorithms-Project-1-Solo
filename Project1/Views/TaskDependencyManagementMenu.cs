using Project1.Models;
using Project1.Models.ViewModels;
using Project1.Services.Interfaces;
using Project1.Views.Mapping;

namespace Project1.Views;

public class TaskDependencyManagementMenu
{
    private ChoiceMenu _menu;
    private TaskDisplayMapper _displayMapper;
    
    public TaskDependencyManagementMenu(TaskDisplayMapper mapper)
    {
        _menu = new ChoiceMenu();
        _displayMapper = mapper;
    }
    
    public void ManageDependencies(IMyCollection<TaskItem> tasks, Func<int, bool> canEdit)
    {
        MenuOption<TaskItem>[] menuItems = BuildTaskSelectionMenuItems(tasks);

        while (true)
        {
            int selectedIndex = DisplayTaskSelectionMenu(menuItems);
            
            if (menuItems[selectedIndex].IsAction)
                return;
            
            if (!canEdit(menuItems[selectedIndex].Value!.Id))
                ManagementBlocked(menuItems[selectedIndex].Value!);
            else
                continue;
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
}