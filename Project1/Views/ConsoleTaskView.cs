using Project1.Models;
using Project1.Models.ViewModels;
using Project1.Services.Interfaces;
using Project1.Views;
using Project1.Views.Mapping;
using Project1.Views.Users;
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
    private readonly AssignTaskMenu _assignTaskMenu;
    private readonly KanbanBoardDisplay _boardDisplay;
    private readonly FiltersMenu _filtersMenu;
    private readonly UserSelectionView _userSelectionView;
    
    private readonly TaskDisplayMapper _displayMapper;
    
    private TaskFilter _filters;
    
    public ConsoleTaskView(ITaskService taskService, IUserService userService, Session session)
    {
        _taskService = taskService;
        _userService = userService;
        _session = session;
        _filters = new TaskFilter();
        
        _menu = new ChoiceMenu();
        _displayMapper = new TaskDisplayMapper(userService);
        _userSelectionView = new UserSelectionView(userService);
        _addUpdateTaskMenu = new AddTaskMenu(session, _userSelectionView);
        _removeTaskMenu = new RemoveTaskMenu(_displayMapper);
        _updateTaskMenu = new UpdateTaskMenu(_displayMapper, _userSelectionView, userService.GetUserById);
        _toggleTaskMenu = new ToggleTaskMenu(_displayMapper);
        _assignTaskMenu = new AssignTaskMenu(_displayMapper, _userSelectionView, userService.GetUserById);
        _boardDisplay = new KanbanBoardDisplay(userService, taskService);
        _filtersMenu = new FiltersMenu(_filters, _userSelectionView, session);
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
                    int taskIdToRemove = _removeTaskMenu.RemoveTask(GetAllTasksFiltered(), CanUserEdit);
                    if (taskIdToRemove != -1)
                        _taskService.RemoveTask(taskIdToRemove, _session.CurrentUser!.Id);
                    break;
                case 2:
                    (int id, UpdateTaskModel updatedTask)? taskToUpdate =
                        _updateTaskMenu.UpdateTask(GetAllTasksFiltered(), CanUserEdit);
                    if (taskToUpdate is not null && taskToUpdate.Value.id != -1)
                        _taskService.UpdateTask(taskToUpdate.Value.id, _session.CurrentUser!.Id,
                            taskToUpdate.Value.updatedTask);
                    break;
                case 3:
                    (int id, TaskStatus status)? taskToToggle =
                        _toggleTaskMenu.ToggleTask(GetAllTasksFiltered(), CanUserEdit);
                    if (taskToToggle is not null && taskToToggle.Value.id != -1)
                        _taskService.ToggleTask(taskToToggle.Value.id, _session.CurrentUser!.Id,
                            taskToToggle.Value.status);
                    
                    break;
                case 4:
                    (int id, int? assigneeId)? taskAssignment =
                        _assignTaskMenu.AssignTask(GetAllTasksFiltered(), _session.CurrentUser!.Id, CanUserEdit);
                    if (taskAssignment is not null)
                        _taskService.AssignTask(taskAssignment.Value.id, _session.CurrentUser!.Id,
                            taskAssignment.Value.assigneeId);
                    break;
                case 5:
                    _filtersMenu.SelectFilters();
                    break;
                
                default:
                    _session.Logout();
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
            "Toggle Task Status",
            "Assign/Reassign Task",
            "Apply filters",
            null,
            "Exit"
        ];
        
        Console.WriteLine("\n=== Options ===\n");
        return _menu.GetChoice(mainMenuOptions);
    }

    private void DisplayActiveFilters()
    {
        if (_filters.IsEmpty && !_filters.ApplySort)
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
        
        if (_filters.Assignee != 0)
            Console.WriteLine($"Assignee    : {_filters.AssigneeUsername}");
        
        if (_filters.ApplySort)
            Console.WriteLine($"Sort        : {_filters.SortBy} ({_filters.SortOrder})");
    }

    private IMyCollection<TaskItem> GetAllTasksFiltered()
    {
        return _taskService.GetAllTasksWithFilter(_filters);
    }

    private bool CanUserEdit(int taskId) => _taskService.CanUserEdit(taskId, _session.CurrentUser!.Id);
    
}