using Project1.Models;
using Project1.Models.ViewModels;
using Project1.Services.Interfaces;
using Project1.Views;

public class ConsoleTaskView : ITaskView
{
    private readonly ITaskService _service;
    private readonly ChoiceMenu<string> _menu;
    private readonly AddTaskMenu _addTaskMenu;
    
    public ConsoleTaskView(ITaskService service)
    {
        _service = service;
        _menu = new ChoiceMenu<string>();
        _addTaskMenu = new AddTaskMenu(_menu);
    }
    void DisplayTasks(IEnumerable<TaskItem> tasks)
    {
        Console.Clear();
        Console.WriteLine("==== ToDo List ====");
        foreach (var task in tasks)
            Console.WriteLine($"{task}");
    }
    string Prompt(string prompt)
    {
        Console.Write(prompt);
        return Console.ReadLine();
    }
    public void Run()
    {
        while (true)
        {
            Console.Clear();
            // DisplayTasks(_service.GetAllTasks());
            int option = MainMenuOption();
            switch (option)
            {
                case 0:
                    CreateTaskInput? newTask = _addTaskMenu.AddTask();
                    // _service.AddTask(description);
                    break;
                case 1:
                    string removeIdStr = Prompt("Enter task id to remove: ");
                    if (int.TryParse(removeIdStr, out int removeId))
                    {
                        _service.RemoveTask(removeId);
                    }
                    break;
                case 2:
                    string toggleIdStr = Prompt("Enter task id to toggle: ");
                    if (int.TryParse(toggleIdStr, out int toggleId))
                    {
                        _service.ToggleTaskCompletion(toggleId);
                    }
                    break;
                case 3:
                    return;
                default:
                    Console.WriteLine("Invalid option. Press any key tocontinue...");
                    Console.ReadKey();
                    break;
            }
        }
    }

    private int MainMenuOption()
    {
        string?[] mainMenuOptions =
        [
            "Add Task",
            "Toggle Task State",
            "Remove Task",
            "Edit Task",
            null,
            "Exit"
        ];
        
        Console.WriteLine("\n=== Options ===\n");
        return _menu.GetChoice(mainMenuOptions);
    }
}