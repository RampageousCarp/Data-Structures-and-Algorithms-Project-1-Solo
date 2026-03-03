using Project1.Models;
using Project1.Services.Interfaces;

namespace Project1.Repositories.Interfaces;
interface ITaskRepository
{
    TaskItem[] LoadTasks();
    void SaveTasks(IMyIterator<TaskItem> tasks, int count);
}