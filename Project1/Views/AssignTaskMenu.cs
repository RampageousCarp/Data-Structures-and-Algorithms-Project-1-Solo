using Project1.Models;
using Project1.Models.ViewModels;
using Project1.Services.Interfaces;
using Project1.Views.Mapping;

namespace Project1.Views;

public class AssignTaskMenu
{
    private readonly ChoiceMenu _menu;
    private readonly TaskDisplayMapper _displayMapper;
    private readonly UserSelectionView _userSelectionView;
    private readonly Func<int, User?> _getUserById;
    
    public AssignTaskMenu(TaskDisplayMapper mapper, UserSelectionView userSelectionView, Func<int, User?> getUserById)
    {
        _menu = new ChoiceMenu();
        _displayMapper = mapper;
        _userSelectionView = userSelectionView;
        _getUserById = getUserById;
    }

    public (int id, int? assigneeId)? AssignTask(IMyCollection<TaskItem> tasks, int currentUserId, Func<int, bool> canEdit)
    {
        MenuOption<TaskItem>[] menuItems = BuildTaskSelectionMenuItems(tasks);
        
        while (true)
        {
            int? newAssignee = -1;
            int selectedIndex = DisplayTaskSelectionMenu(menuItems);
            
            if (menuItems[selectedIndex].IsAction)
                return null;
            
            if (canEdit(menuItems[selectedIndex].Value!.Id))
                newAssignee = ChooseAssignmentMethod(menuItems[selectedIndex].Value!, currentUserId);
            else
                AssignmentBlocked(menuItems[selectedIndex].Value!);

            if (newAssignee != -1 && ConfirmAssignment(menuItems[selectedIndex].Value!, newAssignee is null ? null : _getUserById(newAssignee.GetValueOrDefault())))
                return (menuItems[selectedIndex].Value!.Id, newAssignee);
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
    
    private int? ChooseAssignmentMethod(TaskItem task, int currentUserId)
    {
        string?[] options;
        int menuType;
        if (task.AssignedTo is null)
        {
            menuType = 1;
            options = ["Assign myself", "Assign Task To", null, "Exit"];
        }

        else
        {
            menuType = 2;
            options = ["Unassign myself", "Reassign Task To", null, "Exit"];
        }

        Console.Clear();
        Console.WriteLine($"=== Choose Assignment Option For #{task.Id} {task.Description} ===\n");
        int optionChoice = _menu.GetChoice(options);

        switch (menuType, optionChoice)
        {
            case (1, 0):
                return currentUserId;
            case (1, 1) or (2, 1):
                return AssignTaskTo();
            case (2, 0):
                return null;
            default:
                return -1;
        }
    }

    private int? AssignTaskTo()
    {
        return _userSelectionView.ChooseUser()?.Id ?? -1;
    }
    
    private void AssignmentBlocked(TaskItem task)
    {
        Console.Clear();
        Console.CursorVisible = false;
        Console.WriteLine($"=== Assign/Reassign Task #{task.Id} {task.Description} ===\n");
        Console.WriteLine("You don't have permission to assign/reassign this task");
        Console.WriteLine("Press any key to continue");
        Console.ReadKey();
        Console.CursorVisible = true;
    }
    
    private bool ConfirmAssignment(TaskItem task, User? newAssignee)
    {
        Console.Clear();
        if (newAssignee is null)
            Console.WriteLine($"=== Unassign Task #{task.Id} {task.Description} ===\n");
        else if(task.AssignedTo is not null)
            Console.WriteLine($"=== Reassign Task #{task.Id} {task.Description} To {newAssignee.Username} ===\n");
        else
            Console.WriteLine($"=== Assign Task #{task.Id} {task.Description} To {newAssignee.Username} ===\n");
        
        return _menu.GetChoice(["Yes", "No"]) == 0;
    }
}
