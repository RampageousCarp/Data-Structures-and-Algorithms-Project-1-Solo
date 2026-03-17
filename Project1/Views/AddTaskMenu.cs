using Project1.Models.ENums;
using Project1.Models.ViewModels;
using TaskStatus = Project1.Models.ENums.TaskStatus;

namespace Project1.Views;

public class AddTaskMenu
{
    private readonly ChoiceMenu<string> _menu;

    public AddTaskMenu()
    {
        _menu = new ChoiceMenu<string>();
    }
    
    public CreateTaskModel? AddTask()
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
                case 5:
                    if (!IsValid(newCreateTask))
                        dataIncomplete = true;
                    else
                        return newCreateTask;
                    break;
                case 6:
                    return null;
                default:
                    return null;
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

    private bool IsValid(CreateTaskModel newCreateTask)
    {
        return !string.IsNullOrEmpty(newCreateTask.Description);
    }
}