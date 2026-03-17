using Project1;
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
        Session session = new Session();
        IMyCollectionFactory collectionFactory = new MyArrayCollectionFactory();
        
        string usersFilePath = "Repositories/JSON/users.json";
        IGenericRepository<User> usersRepository = new JsonGenericRepository<User>(usersFilePath);
        User[] loadedUsers = usersRepository.LoadItems();
        IMyIterator<User> usersIterator = new ArrayIterator<User>(loadedUsers, loadedUsers.Length);
        IMyCollection<User> usersCollection = new MyArrayCollection<User>(usersIterator);
        
        IUserService userService = new UserService(usersRepository, usersCollection, collectionFactory);
        
        
        string tasksFilePath = "Repositories/JSON/tasks.json";
        IGenericRepository<TaskItem> tasksRepository = new JsonGenericRepository<TaskItem>(tasksFilePath);
        
        TaskItem[] loadedTasks = tasksRepository.LoadItems();
        IMyIterator<TaskItem> tasksIterator = new ArrayIterator<TaskItem>(loadedTasks, loadedTasks.Length);
        IMyCollection<TaskItem> taskCollection = new MyArrayCollection<TaskItem>(tasksIterator);
        
        ITaskService taskService = new TaskService(tasksRepository, taskCollection, collectionFactory);

        // Run the view
        AppController controller = new AppController(session, userService, taskService);
        controller.Run();
    }
}