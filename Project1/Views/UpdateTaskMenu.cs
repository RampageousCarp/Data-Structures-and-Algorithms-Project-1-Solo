using Project1.Models;
using Project1.Models.ENums;
using Project1.Models.ViewModels;
using Project1.Services.Interfaces;
using Project1.Views.Mapping;
using Project1.Views.Users;
using TaskStatus = Project1.Models.ENums.TaskStatus;

namespace Project1.Views;

public class UpdateTaskMenu
{
    private readonly ChoiceMenu _menu;
    private readonly TaskDisplayMapper _displayMapper;
    private readonly ITaskService _taskService;
    private readonly UserSelectionView _userSelectionView;
    private readonly Func<int, User?> _getUserById;
    
    public UpdateTaskMenu(TaskDisplayMapper mapper, ITaskService taskService, UserSelectionView userSelectionView, Func<int, User?> getUserById)
    {
        _menu = new ChoiceMenu();
        _displayMapper = mapper;
        _taskService = taskService;
        _userSelectionView = userSelectionView;
        _getUserById = getUserById;
    }
    
    public void UpdateTask(Func<IMyCollection<TaskItem>> getTasksWithFilter, int currentUserId, Func<int, bool> canEdit)
    {
        
        while (true)
        {
            MenuOption<TaskItem>[] menuItems = BuildTaskSelectionMenuItems(getTasksWithFilter());
            int selectedIndex = DisplayTaskSelectionMenu(menuItems);
            
            if (menuItems[selectedIndex].IsAction)
                return;

            TaskItem selectedTask = menuItems[selectedIndex].Value!;

            if (!canEdit(selectedTask.Id))
            {
                UpdateBlocked(selectedTask);
                continue;
            }
            
            UpdateTaskModel? updatedTask = HandleTaskUpdate(selectedTask);

            if (updatedTask is not null)
            {
                _taskService.UpdateTask(selectedTask.Id, currentUserId, updatedTask);
                return;
            }
        }
    }
    
    private int DisplayTaskSelectionMenu(MenuOption<TaskItem>[] menuItems)
    {
        Console.Clear();

        int selectedIndex = _menu.GetChoice(menuItems, true, "=== Choose Task To Update ===\n\n");

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
    
    
    private UpdateTaskModel? HandleTaskUpdate(TaskItem taskToUpdate)
    {
        UpdateTaskModel updatedTask = CreateTaskModelFromDisplay(taskToUpdate);
        bool dataIncomplete = false;
        
        while (true)
        {
            int option = DisplayUpdateMenu(taskToUpdate, updatedTask, dataIncomplete);
            dataIncomplete = false;

            switch (option)
            {
                case 0:
                    updatedTask.Description = EnterDescription();
                    break;
                
                case 1:
                    updatedTask.Priority = EnterPriority();
                    break;
                
                case 2:
                    updatedTask.Status = EnterStatus();
                    break;
                
                case 3:
                    updatedTask.DueTo = EnterDueToDate();
                    break;
                case 4:
                    (int id, string name)? newAssignee = ChooseAssignmentAction();
                    if (newAssignee is null)
                        break;
                    if (newAssignee.Value.id == 0)
                    {
                        updatedTask.AssignedTo = null;
                        updatedTask.AssigneeName = newAssignee.Value.name;
                    }
                    else if (newAssignee.Value.id != -1)
                    {
                        updatedTask.AssignedTo = newAssignee.Value.id;
                        updatedTask.AssigneeName = newAssignee.Value.name;
                    }

                    break;
                
                case 6:
                    if (!IsValid(updatedTask))
                    {
                        dataIncomplete = true;
                        continue;
                    }
                    
                    if (ConfirmUpdate(taskToUpdate.Id))
                        return updatedTask;
                    
                    continue;
                
                default:
                    return null;
            }
        }
    }
    
    private int DisplayUpdateMenu(TaskItem original, UpdateTaskModel updated, bool showValidationError)
    {
        Console.Clear();
        Console.WriteLine($"=== Update Task #{original.Id} ===\n");
        
        if (showValidationError)
            Console.WriteLine("Not all fields are filled in!\n");

        string?[] menuItems =
        [
            $"Description: {updated.Description}",
            $"Priority: {updated.Priority}",
            $"Status: {updated.Status}",
            $"Due To: {updated.DueTo:dd-MM-yyyy}",
            $"Assigned To: {updated.AssigneeName}", 
            null,
            "Update",
            "Exit"
        ];

        return _menu.GetChoice(menuItems);
    }
    
    private UpdateTaskModel CreateTaskModelFromDisplay(TaskItem task)
    {
        User? user = _getUserById(task.AssignedTo.GetValueOrDefault());
        return new UpdateTaskModel
        {
            Description = task.Description,
            Priority = task.Priority,
            Status = task.Status,
            DueTo = task.DueTo,
            AssignedTo = task.AssignedTo,
            AssigneeName = task.AssignedTo is null ? "Unassigned" : user!.Username
        };
    }
    
    private string EnterDescription()
    {
        Console.Clear();
        Console.WriteLine("=== Enter Description ===\n");
        Console.Write("Description: ");
        
        return Console.ReadLine() ?? string.Empty;
    }
    
    private TaskPriority EnterPriority()
    {
        Console.Clear();
        Console.WriteLine("=== Enter Priority ===\n");

        string[] priorities = Enum.GetNames(typeof(TaskPriority));
        int selectedIndex = _menu.GetChoice(priorities);

        return (TaskPriority)selectedIndex;
    }
    
    private TaskStatus EnterStatus()
    {
        Console.Clear();
        Console.WriteLine("=== Enter Status ===\n");

        string[] statuses = Enum.GetNames(typeof(TaskStatus));
        int selectedIndex = _menu.GetChoice(statuses);

        return (TaskStatus)selectedIndex;
    }
    
    private DateOnly EnterDueToDate()
    {
        Console.Clear();
        Console.WriteLine("=== Enter Due To Date ===\n");
        Console.Write("DueTo (dd-mm-yyyy): ");

        string? dateString = Console.ReadLine();

        if (DateOnly.TryParseExact(dateString, "dd-MM-yyyy", out DateOnly date))
            return date;
        
        return DateOnly.FromDateTime(DateTime.Now);
    }

    private (int newAssigneeId, string name)? ChooseAssignmentAction()
    {
        Console.Clear();
        Console.WriteLine($"=== Enter new assignee ===\n");
        string?[] choices = ["Unassign", "Assign to", null, "Exit"];
        
        int option = _menu.GetChoice(choices);
        switch (option)
        {
            case 0:
                return (0, "Unassigned");
                
            case 1:
                return ChooseAssignee();
                
            case 3:
                return null;
            
            default:
                return null;
        }
    }

    private (int newAssigneeId, string name)? ChooseAssignee()
    {
        User? chosenUser = _userSelectionView.ChooseUser();
        if (chosenUser is null)
            return null;
        
        return (chosenUser.Id, chosenUser.Username);
    }
    
    private bool IsValid(UpdateTaskModel task)
    {
        return !string.IsNullOrWhiteSpace(task.Description);
    }
    
    private bool ConfirmUpdate(int taskId)
    {
        Console.Clear();
        Console.WriteLine($"=== Confirm Update for Task #{taskId} ===\n");
        Console.WriteLine("Are you sure you want to update this task?\n");

        return _menu.GetChoice(["Yes", "No"]) == 0;
    }

    private void UpdateBlocked(TaskItem task)
    {
        Console.Clear();
        Console.CursorVisible = false;
        Console.WriteLine($"=== Update #{task.Id} ===\n");
        Console.WriteLine("You don't have permission to update this task");
        Console.WriteLine("Press any key to continue");
        Console.ReadKey();
        Console.CursorVisible = true;
    }
}