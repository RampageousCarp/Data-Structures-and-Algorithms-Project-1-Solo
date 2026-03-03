using Project1.Models;
using Project1.Models.ViewModels;

namespace Project1.Services.Interfaces;
public interface ITaskService
{
    IEnumerable<TaskItem> GetAllTasks();
    TaskDisplay[] LoadTasksForDisplay();
    void AddTask(CreateUpdateTaskModel createTaskData);
    void RemoveTask(int id);
    void UpdateTask(int id, CreateUpdateTaskModel updateTaskData);
    void ToggleTaskCompletion(int id);
    void SaveTasks();
}
