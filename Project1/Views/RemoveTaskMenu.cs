using Project1.Models;
using Project1.Services.Interfaces;

namespace Project1.Views;

public class RemoveTaskMenu
{
    private ChoiceMenu<string> _menu;
    
    public RemoveTaskMenu(ChoiceMenu<string> menu)
    {
        _menu = menu;
    }

    public TaskItem? RemoveTask(TaskItem[] tasks)
    {
        string[] itemsToDisplay = new String[tasks.Length + 2];
        
        for (int i = 0; i < tasks.Length; i++)
            itemsToDisplay[i] = $"{tasks[i].Description}";

        itemsToDisplay[^1] = "Exit";

        while (true)
        {
            Console.Clear();

            int taskIndexToRemove = _menu.GetChoice(itemsToDisplay, true, "=== Choose Task To Remove ===\n\n");
            if (taskIndexToRemove == itemsToDisplay.Length - 1)
                return null;
            if (ConfirmRemove(itemsToDisplay[taskIndexToRemove]))
                return tasks[taskIndexToRemove];
        }
    }

    private bool ConfirmRemove(string task)
    {
        Console.Clear();
        Console.WriteLine($"=== Remove {task} ===\n");
        
        return _menu.GetChoice(["Yes", "No"]) == 0;
    }
}