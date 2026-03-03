using Project1.Models;
using Project1.Models.ViewModels;
using TaskStatus = Project1.Models.ENums.TaskStatus;

namespace Project1.Services.Interfaces;
public interface ITaskService
{
    IEnumerable<TaskItem> GetAllTasks();
    TaskDisplay[] LoadTasksForDisplay();
    void AddTask(CreateUpdateTaskModel createTaskData);
    void RemoveTask(int id);
    void UpdateTask(int id, CreateUpdateTaskModel updateTaskData);
    void ToggleTask(int id, TaskStatus newStatus);
    void SaveTasks();
}
