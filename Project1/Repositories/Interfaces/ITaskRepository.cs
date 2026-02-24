using Project1.Models;

namespace Project1.Repositories.Interfaces;
interface ITaskRepository
{
    TaskItem[] LoadTasks();
    void SaveTasks(TaskItem[] tasks, int count);
}