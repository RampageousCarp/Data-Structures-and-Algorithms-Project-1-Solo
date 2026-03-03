using Project1.Models.ENums;
using Project1.Models.ViewModels;
using TaskStatus = Project1.Models.ENums.TaskStatus;

namespace Project1.Views;

public class UpdateTaskMenu
{
    private readonly ChoiceMenu<string> _menu;

    public UpdateTaskMenu(ChoiceMenu<string> menu)
    {
        _menu = menu;
    }
    
    public (int id, CreateUpdateTaskModel updatedTask)? UpdateTask(TaskDisplay[] tasks)
    {
        while (true)
        {
            int selectedIndex = DisplayTaskSelectionMenu(tasks);
            
            if (selectedIndex == -1)
                return null;

            (int id, CreateUpdateTaskModel updatedTask)? result = HandleTaskUpdate(tasks[selectedIndex]);
            if (result.HasValue)
                return result;
        }
    }
    
    private int DisplayTaskSelectionMenu(TaskDisplay[] tasks)
    {
        Console.Clear();

        string[] menuItems = BuildTaskSelectionMenuItems(tasks);
        int selectedIndex = _menu.GetChoice(menuItems, true, "=== Choose Task To Update ===\n\n");

        return selectedIndex == menuItems.Length - 1 ? -1 : selectedIndex;
    }
    
    private string[] BuildTaskSelectionMenuItems(TaskDisplay[] tasks)
    {
        string[] menuItems = new string[tasks.Length + 1];
        
        for (int i = 0; i < tasks.Length; i++)
            menuItems[i] = tasks[i].ToString();

        menuItems[^1] = "Exit";

        return menuItems;
    }
    
    private (int id, CreateUpdateTaskModel updatedTask)? HandleTaskUpdate(TaskDisplay taskToUpdate)
    {
        CreateUpdateTaskModel updatedTask = CreateTaskModelFromDisplay(taskToUpdate);
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
                
                case 4:
                    if (!IsValid(updatedTask))
                    {
                        dataIncomplete = true;
                        break;
                    }
                    
                    if (ConfirmUpdate(taskToUpdate.Id))
                        return (taskToUpdate.Id, updatedTask);
                    
                    return null;
                
                default:
                    return null;
            }
        }
    }
    
    private int DisplayUpdateMenu(TaskDisplay original, CreateUpdateTaskModel updated, bool showValidationError)
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
            null,
            "Update",
            "Exit"
        ];

        return _menu.GetChoice(menuItems);
    }
    
    private CreateUpdateTaskModel CreateTaskModelFromDisplay(TaskDisplay task)
    {
        return new CreateUpdateTaskModel
        {
            Description = task.Description,
            Priority = task.Priority,
            Status = task.Status
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
    
    private bool IsValid(CreateUpdateTaskModel task)
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
}