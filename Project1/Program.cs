using Project1.Models;
using Project1.Repositories;
using Project1.Repositories.Interfaces;
using Project1.Services;
using Project1.Services.Interfaces;

class Program
{
    static void Main(string[] args)
    {
        // Dependency injection: wiring up our components
        string filePath = "Repository/JSON/tasks.json";
        ITaskRepository repository = new JsonTaskRepository(filePath);
        
        TaskItem[] loadedTasks = repository.LoadTasks();
        ITaskService service = new TaskService(repository);
        ITaskView view = new ConsoleTaskView(service);
        // Run the view
        
        view.Run();
    }
}