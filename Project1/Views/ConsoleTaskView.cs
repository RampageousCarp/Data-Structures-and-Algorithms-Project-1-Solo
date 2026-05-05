using Project1.Models;
using Project1.Models.ViewModels;
using Project1.Services.Interfaces;
using Project1.Views.Mapping;
using Project1.Views.Users;

namespace Project1.Views;

public class ConsoleTaskView : ITaskView
{
    private readonly ITaskService _taskService;
    private readonly Session _session;
    private readonly ChoiceMenu _menu;
    private readonly AddTaskMenu _addTaskMenu;
    private readonly RemoveTaskMenu _removeTaskMenu;
    private readonly UpdateTaskMenu _updateTaskMenu;
    private readonly ToggleTaskMenu _toggleTaskMenu;
    private readonly AssignTaskMenu _assignTaskMenu;
    private readonly TaskDependencyManagementMenu _dependencyManagementMenu;
    private readonly KanbanBoardDisplay _boardDisplay;
    private readonly FiltersMenu _filtersMenu;

    private TaskFilter _filters;
    
    public ConsoleTaskView(ITaskService taskService, IUserService userService, Session session)
    {
        TaskDisplayMapper displayMapper = new TaskDisplayMapper(userService);
        UserSelectionView userSelectionView = new UserSelectionView(userService);
        
        _menu = new ChoiceMenu();
        _session = session;
        _filters = new TaskFilter();
        _taskService = taskService;
        
        _addTaskMenu = new AddTaskMenu(session, taskService, userSelectionView);
        _removeTaskMenu = new RemoveTaskMenu(displayMapper, taskService);
        _updateTaskMenu = new UpdateTaskMenu(displayMapper, taskService, userSelectionView, userService.GetUserById);
        _toggleTaskMenu = new ToggleTaskMenu(displayMapper, taskService);
        _dependencyManagementMenu = new TaskDependencyManagementMenu(displayMapper, taskService);
        _assignTaskMenu = new AssignTaskMenu(displayMapper, taskService, userSelectionView, userService.GetUserById);
        _boardDisplay = new KanbanBoardDisplay(userService, taskService);
        _filtersMenu = new FiltersMenu(_filters, userSelectionView, session);
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
                    _addTaskMenu.AddTask();
                    break;
                case 1:
                    _removeTaskMenu.RemoveTask(GetAllTasksFiltered, _session.CurrentUser!.Id, CanUserEdit);
                    break;
                case 2:
                    _updateTaskMenu.UpdateTask(GetAllTasksFiltered, _session.CurrentUser!.Id, CanUserEdit);
                    // (int id, UpdateTaskModel updatedTask)? taskToUpdate =
                    //     _updateTaskMenu.UpdateTask(GetAllTasksFiltered(), CanUserEdit);
                    // if (taskToUpdate is not null && taskToUpdate.Value.id != -1)
                    //     _taskService.UpdateTask(taskToUpdate.Value.id, _session.CurrentUser!.Id,
                    //         taskToUpdate.Value.updatedTask);
                    break;
                case 3:
                    _toggleTaskMenu.ToggleTask(GetAllTasksFiltered, _session.CurrentUser!.Id, CanUserEdit);
                    break;
                case 4:
                    _assignTaskMenu.AssignTask(GetAllTasksFiltered, _session.CurrentUser!.Id, CanUserEdit);
                    break;
                case 5:
                    _dependencyManagementMenu.ManageDependencies(GetAllTasksFiltered, CanUserEdit);
                    break;
                case 6:
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
            "Manage Task Dependencies",
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