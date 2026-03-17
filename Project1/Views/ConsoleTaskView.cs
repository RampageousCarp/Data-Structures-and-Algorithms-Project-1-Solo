using Project1.Models;
using Project1.Models.ViewModels;
using Project1.Services.Interfaces;
using Project1.Views;
using TaskStatus = Project1.Models.ENums.TaskStatus;

public class ConsoleTaskView : ITaskView
{
    private readonly ITaskService _taskService;
    private readonly IUserService _userService;
    private readonly Session _session;
    private readonly ChoiceMenu<string> _menu;
    private readonly AddTaskMenu _addUpdateTaskMenu;
    private readonly RemoveTaskMenu _removeTaskMenu;
    private readonly UpdateTaskMenu _updateTaskMenu;
    private readonly ToggleTaskMenu _toggleTaskMenu;
    private readonly KanbanBoardDisplay _boardDisplay;
    private readonly FiltersMenu _filtersMenu;
    private readonly UserSelectionView _userSelectionView;
    
    private TaskFilter _filters;
    
    public ConsoleTaskView(ITaskService taskService, IUserService userService, Session session)
    {
        _taskService = taskService;
        _userService = userService;
        _session = session;
        _filters = new TaskFilter();
        
        _menu = new ChoiceMenu<string>();
        _addUpdateTaskMenu = new AddTaskMenu();
        _removeTaskMenu = new RemoveTaskMenu();
        _updateTaskMenu = new UpdateTaskMenu();
        _toggleTaskMenu = new ToggleTaskMenu();
        _boardDisplay = new KanbanBoardDisplay();
        _filtersMenu = new FiltersMenu(_filters);
        _userSelectionView = new UserSelectionView(_userService);

    }

    public void Run()
    {
        while (true)
        {
            Console.Clear();
            GroupedTasks groupedTasks = _taskService.GetGroupedTasks(_filters);
            _boardDisplay.DisplayKanbanBoard(groupedTasks);
            int option = MainMenuOption();
            switch (option)
            {
                case 0:
                    CreateTaskModel? newTask = _addUpdateTaskMenu.AddTask();
                    if (newTask is not null)
                        _taskService.AddTask(newTask);
                    break;
                case 1:
                    int taskIdToRemove = _removeTaskMenu.RemoveTask(LoadAllDisplayTasks());
                    if (taskIdToRemove != -1)
                        _taskService.RemoveTask(taskIdToRemove);
                    break;
                case 2:
                    (int id, UpdateTaskModel updatedTask)? taskToUpdate = _updateTaskMenu.UpdateTask(LoadAllDisplayTasks());
                    
                    if (taskToUpdate is not null && taskToUpdate.Value.id != -1)
                        _taskService.UpdateTask(taskToUpdate.Value.id, taskToUpdate.Value.updatedTask);
                    
                    break;
                case 3:
                    (int id, TaskStatus status)? taskToToggle = _toggleTaskMenu.ToggleTask(LoadAllDisplayTasks());
                    if (taskToToggle is not null && taskToToggle.Value.id != -1)
                        _taskService.ToggleTask(taskToToggle.Value.id, taskToToggle.Value.status);
                    
                    break;
                case 4:
                    _filtersMenu.SelectFilters();
                    break;
                case 6:
                    User? newLogin = _userSelectionView.ChooseUser();
                    if (newLogin is not null)
                        _session.Login(newLogin);
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
            "Apply filters",
            null,
            "Change user",
            "Exit"
        ];
        
        Console.WriteLine("\n=== Options ===\n");
        return _menu.GetChoice(mainMenuOptions);
    }

    private TaskDisplay[] LoadAllDisplayTasks()
    {
        TaskItem[] tasks = _taskService.GetAllTasksSorted(_filters);
        TaskDisplay[] displayTasks = new TaskDisplay[tasks.Length];

        for (int i = 0; i < tasks.Length; i++)
            displayTasks[i] = TaskDisplay.FromTask(tasks[i]);

        return displayTasks;
    }
}