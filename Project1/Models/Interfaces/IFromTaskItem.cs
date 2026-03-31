namespace Project1.Models.Interfaces;

public interface IFromTaskItem<T>
{
    static abstract T FromTask(TaskItem task);
}