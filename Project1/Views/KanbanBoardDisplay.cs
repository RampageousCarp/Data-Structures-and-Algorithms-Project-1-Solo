using Project1.Models;
using Project1.Models.ViewModels;
using Project1.Services.Interfaces;

namespace Project1.Views;

public class KanbanBoardDisplay
{
    private const int TABLE_WIDTH = 162;
    private const int COLUMN_WIDTH = 54;
    
    private static readonly Func<TaskItem, string>[] _fieldExtractors =
    [
        t => t.ConvertTo<TaskTableView>().ToTableId(),
        t => t.ConvertTo<TaskTableView>().ToTableDescription(),
        t => t.ConvertTo<TaskTableView>().ToTablePrio(),
        t => t.ConvertTo<TaskTableView>().ToTableDueTo(),
        t => t.ConvertTo<TaskTableView>().ToTableCreated(),
    ];
    
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
        Console.WriteLine(new string(' ', 25) + "DONE" + new string(' ', 25));
        
        DisplayEndLine();
    }
    
    private void DisplayTasks(GroupedTasks groupedTasks)
    {
        IMyIterator<TaskItem> todoIt = groupedTasks.Todo.GetIterator();
        IMyIterator<TaskItem> inProgressIt = groupedTasks.InProgress.GetIterator();
        IMyIterator<TaskItem> doneIt = groupedTasks.Done.GetIterator();
 
        while(todoIt.HasNext() || inProgressIt.HasNext() || doneIt.HasNext())
        {
            TaskItem? todo = todoIt.HasNext() ? todoIt.Next() : null;
            TaskItem? inProgress = inProgressIt.HasNext() ? inProgressIt.Next() : null;
            TaskItem? done = doneIt.HasNext() ? doneIt.Next() : null;
 
            int rowTop = Console.CursorTop;
 
            for (int i = 0; i < _fieldExtractors.Length; i++)
            {
                Console.SetCursorPosition(0, rowTop + i);
                DisplayTaskField(todo is not null ? _fieldExtractors[i](todo) : "           ");
                DisplayTaskField(inProgress is not null ? _fieldExtractors[i](inProgress) : "");
                DisplayTaskField(done is not null ? _fieldExtractors[i](done) : "");
                Console.WriteLine();
            }
 
            Console.SetCursorPosition(0, rowTop + _fieldExtractors.Length);
            DisplayEndLine();
        }
    }
    
    private void DisplayTaskField(string text)
    {
        if (text.Length >= COLUMN_WIDTH)
            Console.Write(text.Substring(0, COLUMN_WIDTH - 1) + " ");
        else
            Console.Write(text + new string(' ', COLUMN_WIDTH - text.Length));
    }

    private void DisplayEndLine()
    {
        Console.WriteLine(new string('-', TABLE_WIDTH - 1));
    }
}