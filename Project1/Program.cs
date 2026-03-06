using Project1.Models;
using Project1.Repositories;
using Project1.Repositories.Interfaces;
using Project1.Services;
using Project1.Services.Collections;
using Project1.Services.Factories;
using Project1.Services.Interfaces;

class Program
{
    static void Main(string[] args)
    {
        // Dependency injection: wiring up our components
        string filePath = "Repositories/JSON/tasks.json";
        ITaskRepository repository = new JsonTaskRepository(filePath);

        TaskItem[] loadedTasks = repository.LoadTasks();
        IMyIterator<TaskItem> iterator = new ArrayIterator<TaskItem>(loadedTasks, loadedTasks.Length);

        MyArrayCollection<TaskItem> taskCollection = new MyArrayCollection<TaskItem>(iterator);
        IMyCollectionFactory collectionFactory = new MyArrayCollectionFactory();

        ITaskService service = new TaskService(repository, taskCollection, collectionFactory);
        ITaskView view = new ConsoleTaskView(service);
        // Run the view

        view.Run();
    }
}