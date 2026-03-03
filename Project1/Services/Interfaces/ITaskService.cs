using Project1.Models;
using Project1.Models.ViewModels;

namespace Project1.Services.Interfaces;
public interface ITaskService
{
    IEnumerable<TaskItem> GetAllTasks();
    void AddTask(CreateTaskInput taskData);
    void RemoveTask(int id);
    void ToggleTaskCompletion(int id);
}
