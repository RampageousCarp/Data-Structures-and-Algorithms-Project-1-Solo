using Project1.Models.ViewModels;
using Project1.Services.Interfaces;

namespace Project1.Views;

public class KanbanBoardDisplay
{
    private ITaskService _service;
    private const int TABLE_WIDTH = 162;

    public KanbanBoardDisplay(ITaskService service)
    {
        _service = service;
    }
    
    public void DisplayKanbanBoard(GroupedTasks groupedTasks)
    {
        Console.Clear();
        DisplayTitle();
        DisplayHeaders();
        DisplayTasks(groupedTasks);
    }

    private void DisplayTitle()
    {
        Console.WriteLine(new string('=', TABLE_WIDTH));
        Console.WriteLine('|' + new string(' ', TABLE_WIDTH / 2 - 7) + "KANBAN BOARD" + new string(' ', TABLE_WIDTH / 2 - 7) + '|');
        Console.WriteLine(new string('=', TABLE_WIDTH));
    }

    private void DisplayHeaders()
    {
        Console.WriteLine();
        Console.Write(new string(' ', 25) + "TO DO" + new string(' ', 24));
        Console.Write(new string(' ', 22) + "IN PROGRESS" + new string(' ', 21));
        Console.Write(new string(' ', 25) + "DONE" + new string(' ', 25));
        
        DisplayEndLine();
    }
    
    private void DisplayTasks(GroupedTasks groupedTasks)
    {
        int maxLength = Math.Max(groupedTasks.Todo.Length,
            Math.Max(groupedTasks.InProgress.Length, groupedTasks.Done.Length));

        for (int i = 0; i < maxLength; i++)
        {
            int rowTop = Console.CursorTop;

            // ID
            Console.SetCursorPosition(0, rowTop);
            if (groupedTasks.Todo.Length > i)
                DisplayTaskField(groupedTasks.Todo[i].ToTableId());
            else
                DisplayTaskField("");
            
            if (groupedTasks.InProgress.Length > i)
                DisplayTaskField(groupedTasks.InProgress[i].ToTableId());
            else
                DisplayTaskField("");
            
            if (groupedTasks.Done.Length > i)
                DisplayTaskField(groupedTasks.Done[i].ToTableId());
            else
                DisplayTaskField("");
            
            Console.WriteLine();
            
            // Description
            Console.SetCursorPosition(0, rowTop + 1);
            if (groupedTasks.Todo.Length > i)
                DisplayTaskField(groupedTasks.Todo[i].ToTableDescription());
            else
                DisplayTaskField("");
            
            if (groupedTasks.InProgress.Length > i)
                DisplayTaskField(groupedTasks.InProgress[i].ToTableDescription());
            else
                DisplayTaskField("");
            
            if (groupedTasks.Done.Length > i)
                DisplayTaskField(groupedTasks.Done[i].ToTableDescription());
            else
                DisplayTaskField("");
            
            Console.WriteLine();
            
            // Priority
            Console.SetCursorPosition(0, rowTop + 2);
            if (groupedTasks.Todo.Length > i)
                DisplayTaskField(groupedTasks.Todo[i].ToTablePrio());
            else
                DisplayTaskField("");
            
            if (groupedTasks.InProgress.Length > i)
                DisplayTaskField(groupedTasks.InProgress[i].ToTablePrio());
            else
                DisplayTaskField("");
            
            if (groupedTasks.Done.Length > i)
                DisplayTaskField(groupedTasks.Done[i].ToTablePrio());
            else
                DisplayTaskField("");
            
            Console.WriteLine();
            
            // Created at
            Console.SetCursorPosition(0, rowTop + 3);
            if (groupedTasks.Todo.Length > i)
                DisplayTaskField(groupedTasks.Todo[i].ToTableCreated());
            else
                DisplayTaskField("");
            
            if (groupedTasks.InProgress.Length > i)
                DisplayTaskField(groupedTasks.InProgress[i].ToTableCreated());
            else
                DisplayTaskField("");
            
            if (groupedTasks.Done.Length > i)
                DisplayTaskField(groupedTasks.Done[i].ToTableCreated());
            else
                DisplayTaskField("");
            
            Console.WriteLine();

            Console.SetCursorPosition(0, rowTop + 4);
            DisplayEndLine();
        }
    }

    private void DisplayTaskField(string text)
    {
        if (text.Length >= 54)
            Console.Write(text.Substring(0, 53) + " ");
        else
            Console.Write(text + new string(' ', 54 - text.Length));
    }

    private void DisplayEndLine()
    {
        Console.WriteLine("\n" + new string('-', TABLE_WIDTH));
    }
}