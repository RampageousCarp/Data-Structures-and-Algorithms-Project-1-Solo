using Project1.Models;
using Project1.Models.ViewModels;
using Project1.Services.Interfaces;

namespace Project1.Views;

public class RemoveTaskMenu
{
    private ChoiceMenu _menu;
    
    public RemoveTaskMenu()
    {
        _menu = new ChoiceMenu();
    }

    public int RemoveTask(IMyCollection<TaskItem> tasks)
    {
        MenuOption<TaskItem>[] itemsToDisplay = new MenuOption<TaskItem>[tasks.Count + 1];

        IMyIterator<TaskItem> iterator = tasks.GetIterator();
        int p = 0;
        while (iterator.HasNext())
        {
            TaskItem task = iterator.Next();
            itemsToDisplay[p++] = new MenuOption<TaskItem>(task, task.ConvertTo<TaskDisplay>().ToString());
        }
        
        itemsToDisplay[^1] = new MenuOption<TaskItem>("Exit");

        while (true)
        {
            Console.Clear();

            int taskIndexToRemove = _menu.GetChoice(itemsToDisplay, true, "=== Choose Task To Remove ===\n\n");
            if (itemsToDisplay[taskIndexToRemove].IsAction)
                return -1;
            if (ConfirmRemove(itemsToDisplay[taskIndexToRemove].Value!.ConvertTo<TaskDisplay>()))
                return itemsToDisplay[taskIndexToRemove].Value!.Id;
        }
    }

    private bool ConfirmRemove(TaskDisplay task)
    {
        Console.Clear();
        Console.WriteLine($"=== Remove #{task.Id} {task.Description} ===\n");
        
        return _menu.GetChoice(["Yes", "No"]) == 0;
    }
}