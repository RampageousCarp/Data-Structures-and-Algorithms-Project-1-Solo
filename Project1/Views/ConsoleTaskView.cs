using Project1.Models;
using Project1.Models.ViewModels;
using Project1.Services.Interfaces;
using Project1.Views;
using TaskStatus = Project1.Models.ENums.TaskStatus;

public class ConsoleTaskView : ITaskView
{
    private readonly ITaskService _service;
    private readonly ChoiceMenu<string> _menu;
    private readonly AddTaskMenu _addUpdateTaskMenu;
    private readonly RemoveTaskMenu _removeTaskMenu;
    private readonly UpdateTaskMenu _updateTaskMenu;
    private readonly ToggleTaskMenu _toggleTaskMenu;
    
    public ConsoleTaskView(ITaskService service)
    {
        _service = service;
        _menu = new ChoiceMenu<string>();
        _addUpdateTaskMenu = new AddTaskMenu(_menu);
        _removeTaskMenu = new RemoveTaskMenu(_menu);
        _updateTaskMenu = new UpdateTaskMenu(_menu);
        _toggleTaskMenu = new ToggleTaskMenu(_menu);
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
                    CreateUpdateTaskModel? newTask = _addUpdateTaskMenu.AddTask();
                    if (newTask is not null)
                        _service.AddTask(newTask);
                    break;
                case 1:
                    TaskDisplay[] tasksToDisplayDelete = _service.LoadTasksForDisplay();
                    int taskIdToRemove = _removeTaskMenu.RemoveTask(tasksToDisplayDelete);
                    if (taskIdToRemove != -1)
                        _service.RemoveTask(taskIdToRemove);
                    break;
                case 2:
                    TaskDisplay[] tasksToDisplayUpdate = _service.LoadTasksForDisplay();
                    (int id, CreateUpdateTaskModel updatedTask)? taskToUpdate = _updateTaskMenu.UpdateTask(tasksToDisplayUpdate);
                    
                    if (taskToUpdate is not null && taskToUpdate.Value.id != -1)
                    {
                        _service.UpdateTask(taskToUpdate.Value.id, taskToUpdate.Value.updatedTask);
                    }
                    break;
                case 3:
                    TaskDisplay[] tasksToDisplayToggle = _service.LoadTasksForDisplay();
                    (int id, TaskStatus status)? taskToToggle = _toggleTaskMenu.ToggleTask(tasksToDisplayToggle);
                    if (taskToToggle is not null && taskToToggle.Value.id != -1)
                    {
                        _service.ToggleTask(taskToToggle.Value.id, taskToToggle.Value.status);
                    }
                    break;
                
                default:
                    return;
            }
        }
    }

    private int MainMenuOption()
    {
        string?[] mainMenuOptions =
        [
            "Add Task",
            "Remove Task",
            "Update Task",
            "Toggle Task State",
            null,
            "Exit"
        ];
        
        Console.WriteLine("\n=== Options ===\n");
        return _menu.GetChoice(mainMenuOptions);
    }
}