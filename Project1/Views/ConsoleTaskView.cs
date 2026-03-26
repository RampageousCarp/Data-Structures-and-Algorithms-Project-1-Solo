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
    private readonly ChoiceMenu _menu;
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
        
        _menu = new ChoiceMenu();
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
            DisplayActiveFilters();
                
            int option = MainMenuOption();
            switch (option)
            {
                case 0:
                    CreateTaskModel? newTask = _addUpdateTaskMenu.AddTask();
                    if (newTask is not null)
                        _taskService.AddTask(newTask);
                    break;
                case 1:
                    int taskIdToRemove = _removeTaskMenu.RemoveTask(GetAllTasksFiltered());
                    if (taskIdToRemove != -1)
                        _taskService.RemoveTask(taskIdToRemove);
                    break;
                case 2:
                    (int id, UpdateTaskModel updatedTask)? taskToUpdate = _updateTaskMenu.UpdateTask(GetAllTasksFiltered());
                    
                    if (taskToUpdate is not null && taskToUpdate.Value.id != -1)
                        _taskService.UpdateTask(taskToUpdate.Value.id, taskToUpdate.Value.updatedTask);
                    
                    break;
                // case 3:
                //     (int id, TaskStatus status)? taskToToggle = _toggleTaskMenu.ToggleTask(GetAllTasksFiltered());
                //     if (taskToToggle is not null && taskToToggle.Value.id != -1)
                //         _taskService.ToggleTask(taskToToggle.Value.id, taskToToggle.Value.status);
                    
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
                    Environment.Exit(0);
                    break;
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

    private void DisplayActiveFilters()
    {
        if (_filters.IsEmpty)
            return;
        
        Console.WriteLine("\n=== Active filters ===");
        
        if (_filters.Priority is not null)
            Console.WriteLine($"Priority    : {_filters.Priority}");
        if (_filters.DueToFrom is not null || _filters.DueToTo is not null)
        {
            string dueToFilter = "";

            if (_filters.DueToFrom is not null)
                dueToFilter += $"From: {_filters.DueToFrom:dd-MM-yyyy} ";
        
            if(_filters.DueToTo is not null)
                dueToFilter += $"To: {_filters.DueToTo:dd-MM-yyyy}";

            Console.WriteLine($"Due Date    : {dueToFilter}");
        }
        
        if (_filters.CreatedAtFrom is not null || _filters.CreatedAtTo is not null)
        {
            string createdAtFilter = "";

            if (_filters.CreatedAtFrom is not null)
                createdAtFilter += $"From: {_filters.CreatedAtFrom:dd-MM-yyyy} ";
        
            if(_filters.CreatedAtTo is not null)
                createdAtFilter += $"To: {_filters.CreatedAtTo:dd-MM-yyyy}";

            Console.WriteLine($"Created at  : {createdAtFilter}");
        }
        
        if (!string.IsNullOrEmpty(_filters.Keyword))
            Console.WriteLine($"Keyword     : \"{_filters.Keyword}\"");
        
        if (_filters.ApplySort)
            Console.WriteLine($"Sort        : {_filters.SortBy} ({_filters.SortOrder})");
    }

    private IMyCollection<TaskItem> GetAllTasksFiltered()
    {
        return _taskService.GetAllTasksWithFilter(_filters);
    }
}