using Project1.Models;
using Project1.Models.ViewModels;
using TaskStatus = Project1.Models.ENums.TaskStatus;

namespace Project1.Services.Interfaces;
public interface ITaskService
{
    IEnumerable<TaskItem> GetAllTasks();
    TaskItem[] GetAllTasksSorted(TaskFilter filter);
    TaskItem GetTasks(TaskFilter? filter);
    GroupedTasks GetGroupedTasks(TaskFilter? filter);
    void AddTask(CreateTaskModel createTaskData);
    void RemoveTask(int id);
    void UpdateTask(int id, UpdateTaskModel updateTaskData);
    void ToggleTask(int id, TaskStatus newStatus);
    void SaveTasks();
}
