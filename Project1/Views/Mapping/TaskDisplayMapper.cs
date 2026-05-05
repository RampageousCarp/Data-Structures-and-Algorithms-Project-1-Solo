using Project1.Models;
using Project1.Models.ViewModels;
using Project1.Services.Interfaces;

namespace Project1.Views.Mapping;

public class TaskDisplayMapper
{
    private readonly IUserService _userService;

    public TaskDisplayMapper(IUserService userService)
    {
        _userService = userService;
    }

    public TaskDisplay Map(TaskItem task)
    {
        User? user = _userService.GetUserById(task.AssignedTo.GetValueOrDefault());

        return new TaskDisplay
        {
            Id = task.Id,
            Description = task.Description,
            Priority = task.Priority,
            Status = task.Status,
            DueTo = task.DueTo,
            AssigneeName = user?.Username ?? "Unassigned",
            DependsOn = task.DependsOn,
        };
    }
}