using Project1;
using Project1.Models;
using Project1.Repositories;
using Project1.Repositories.Interfaces;
using Project1.Services;
using Project1.Services.Collections;
using Project1.Services.Factories;
using Project1.Services.Interfaces;
using Project1.Views;

class Program
{
    static void Main(string[] args)
    {
        // Dependency injection: wiring up our components
        Session session = new Session();
        string[] collectionTypes = ["Array", "Linked List", "Hash Map", "Binary Tree"];
        int collectionChoice = new ChoiceMenu().GetChoice(collectionTypes, true, $"=== Choose Collection Type ===\n\n");
        
        IMyCollectionFactory collectionFactory = collectionChoice switch
        {
            0 => new MyArrayCollectionFactory(),
            1 => new MyLinkedListCollectionFactory(),
            2 => new MyHashMapCollectionFactory(),
            // 3 => new MyBinaryTreeCollectionFactory(),
        };
        
        string usersFilePath = "Repositories/JSON/users.json";
        IGenericRepository<User> usersRepository = new JsonGenericRepository<User>(usersFilePath);
        User[] loadedUsers = usersRepository.LoadItems();
        IMyIterator<User> usersIterator = new ArrayIterator<User>(loadedUsers, loadedUsers.Length);
        IMyCollection<User> usersCollection = collectionFactory.Create(usersIterator);
        
        IUserService userService = new UserService(usersRepository, usersCollection, collectionFactory);
        
        
        string tasksFilePath = "Repositories/JSON/tasks.json";
        IGenericRepository<TaskItem> tasksRepository = new JsonGenericRepository<TaskItem>(tasksFilePath);
        
        TaskItem[] loadedTasks = tasksRepository.LoadItems();
        IMyIterator<TaskItem> tasksIterator = new ArrayIterator<TaskItem>(loadedTasks, loadedTasks.Length);
        IMyCollection<TaskItem> taskCollection = collectionFactory.Create(tasksIterator);
        
        ITaskService taskService = new TaskService(tasksRepository, taskCollection, collectionFactory, userService);

        // Run the view
        AppController controller = new AppController(session, userService, taskService);
        controller.Run();
    }
}