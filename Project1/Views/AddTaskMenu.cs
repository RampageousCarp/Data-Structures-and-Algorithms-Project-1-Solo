using Project1.Models.ENums;
using Project1.Models.ViewModels;
using TaskStatus = Project1.Models.ENums.TaskStatus;

namespace Project1.Views;

public class AddTaskMenu
{
    private readonly ChoiceMenu<string> _menu;

    public AddTaskMenu(ChoiceMenu<string> menu)
    {
        _menu = menu;
    }
    
    public CreateTaskModel? AddTask()
    {
        CreateTaskModel newUpdateTask = new CreateTaskModel();
        bool dataIncomplete = false;
        
        while (true)
        {
            string?[] fieldsToEnter =
            [
                $"Description: {newUpdateTask.Description}",
                $"Priority: {newUpdateTask.Priority}",
                $"Status: {newUpdateTask.Status}",
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
                    newUpdateTask.Description = EnterDescription();
                    break;
                case 1:
                    newUpdateTask.Priority = EnterPriority();
                    break;
                case 2:
                    newUpdateTask.Status = EnterStatus();
                    break;
                case 4:
                    if (!IsValid(newUpdateTask))
                        dataIncomplete = true;
                    else
                        return newUpdateTask;
                    break;
                case 5:
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

    private bool IsValid(CreateTaskModel newUpdateTask)
    {
        return !string.IsNullOrEmpty(newUpdateTask.Description);
    }
}