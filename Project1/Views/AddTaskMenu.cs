using Project1.Models;
using Project1.Models.ENums;
using Project1.Models.ViewModels;
using Project1.Services.Interfaces;
using Project1.Views.Users;
using TaskStatus = Project1.Models.ENums.TaskStatus;

namespace Project1.Views;

public class AddTaskMenu
{
    private readonly ChoiceMenu _menu;
    private readonly UserSelectionView _userSelectionView;
    private readonly ITaskService _taskService;
    private readonly Session _session;

    public AddTaskMenu(Session session, ITaskService taskService, UserSelectionView userSelectionView)
    {
        _menu = new ChoiceMenu();
        
        _session = session;
        _taskService = taskService;
        _userSelectionView = userSelectionView;
    }
    
    public void AddTask()
    {
        CreateTaskModel newCreateTask = new CreateTaskModel();
        bool dataIncomplete = false;
        
        while (true)
        {
            string?[] fieldsToEnter =
            [
                $"Description: {newCreateTask.Description}",
                $"Priority: {newCreateTask.Priority}",
                $"Status: {newCreateTask.Status}",
                $"Due To: {newCreateTask.DueTo:dd-MM-yyyy}",
                $"Assigned To: {newCreateTask.AssigneeName}",
                null,
                "Add",
                "Exit"
            ];

            Console.Clear();
            Console.WriteLine("=== Add New Task ===\n");
            if (dataIncomplete)
                Console.WriteLine("Not all fields are filled in!\n");
            dataIncomplete = false;

            
            int option = _menu.GetChoice(fieldsToEnter);

            switch (option)
            {
                case 0:
                    newCreateTask.Description = EnterDescription();
                    break;
                case 1:
                    newCreateTask.Priority = EnterPriority();
                    break;
                case 2:
                    newCreateTask.Status = EnterStatus();
                    break;
                case 3:
                    newCreateTask.DueTo = EnterDueToDate();
                    break;
                case 4:
                    (int id, string name)? assignee = ChooseAssignmentAction();
                    if (assignee!.Value.id == 0)
                    {
                        newCreateTask.AssignedTo = null;
                        newCreateTask.AssigneeName = assignee.Value.name;
                    }
                    else if (assignee.Value.id != -1)
                    {
                        newCreateTask.AssignedTo = assignee.Value.id;
                        newCreateTask.AssigneeName = assignee.Value.name;
                    }
                    break;
                case 6:
                    if (!IsValid(newCreateTask))
                    {
                        dataIncomplete = true;
                        continue;
                    }
                    
                    _taskService.AddTask(newCreateTask);
                    return;
                case 7:
                    return;
                default:
                    return;
            }
        }
    }

    private string EnterDescription()
    {
        Console.Clear();
        Console.WriteLine("=== Enter Description ===\n");
        Console.Write("Description: ");
        
        string? description = Console.ReadLine();

        return description ?? "";
    }

    private TaskPriority EnterPriority()
    {
        string[] priorities = Enum.GetNames(typeof(TaskPriority));
        
        Console.Clear();
        Console.WriteLine("=== Enter Priority ===\n");

        return (TaskPriority)_menu.GetChoice(priorities);
    }
    
    private TaskStatus EnterStatus()
    {
        string[] statuses = Enum.GetNames(typeof(TaskStatus));
        
        Console.Clear();
        Console.WriteLine("=== Enter Status ===\n");

        return (TaskStatus)_menu.GetChoice(statuses);
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
        string?[] choices = ["Unassigned", "Myself", "Another User", null, "Exit"];
        
        int option = _menu.GetChoice(choices);
        switch (option)
        {
            case 0:
                return (0, "Unassigned");
                break;
            case 1:
                return (_session.CurrentUser!.Id, _session.CurrentUser.Username);
                
            case 2:
                return ChooseAssignee();
                
            case 4:
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

    private bool IsValid(CreateTaskModel newCreateTask)
    {
        return !string.IsNullOrEmpty(newCreateTask.Description);
    }
}