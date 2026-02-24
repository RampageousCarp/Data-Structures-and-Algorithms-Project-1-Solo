using System.Text.Json;
using Project1.Models;
using Project1.Repositories.Interfaces;

namespace Project1.Repositories;
class JsonTaskRepository : ITaskRepository
{
    private readonly string _filePath;
    public JsonTaskRepository(string filePath) => _filePath = filePath;
    public TaskItem[] LoadTasks()
    {
        if (!File.Exists(_filePath))
        {
            return new TaskItem[0];
        }
        string json = File.ReadAllText(_filePath);
        
        var tasksList = JsonSerializer.Deserialize<List<TaskItem>>(json);
        
        if (tasksList == null || tasksList.Count == 0) {
            return new TaskItem[0];
        }
        
        TaskItem[] tasks = new TaskItem[tasksList.Count];
        for (int i = 0; i < tasksList.Count; i++)
            tasks[i] = tasksList[i];
        
        return tasks;
    }
    public void SaveTasks(TaskItem[] tasks, int count)
    {
        List<TaskItem> tasksList = new List<TaskItem>();
        for (int i = 0; i < count; i++)
            tasksList.Add(tasks[i]);
        
        string json = JsonSerializer.Serialize(tasksList, new JsonSerializerOptions { 
            WriteIndented = true 
        });
        
        File.WriteAllText(_filePath, json);
    }
}