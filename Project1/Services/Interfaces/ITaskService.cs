using Project1.Models;
using Project1.Models.ViewModels;
using TaskStatus = Project1.Models.ENums.TaskStatus;

namespace Project1.Services.Interfaces;
public interface ITaskService
{
    IMyCollection<TaskItem> GetAllTasksWithFilter(TaskFilter filter);
    GroupedTasks GetGroupedTasks(TaskFilter? filter);
    void AddTask(CreateTaskModel createTaskData);
    bool RemoveTask(int id, int currentUserId);
    bool UpdateTask(int id, int currentUserId, UpdateTaskModel updateTaskData);
    bool ToggleTask(int id, int currentUserId, TaskStatus newStatus);
    bool AssignTask(int id, int currentUserId, int? newAssignee);
    void UnassignUser(int userId);
    bool CanUserEdit(int taskId, int currentUserId);
    void SaveTasks();
    bool IsBlocked(int taskId);
    int[] GetBlockingTasksIds(int taskId);
    IMyCollection<TaskItem> GetAllDependencyTasks(int taskId);
    void RemoveDependency(int taskId, int dependencyTaskId);
    void RemoveAllDependencies(int taskId);
    void AddDependency(int taskId, int dependencyId);
    bool WouldCreateCycle(int taskId, int dependencyId);
    bool AlreadyInDependsOn(int taskId, int dependencyId);
    void SetTaskInToDo(int taskId);
}
