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
    
    public void DisplayKanbanBoard()
    {
        Console.Clear();
        DisplayTitle();
        DisplayHeaders();
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

    private void DisplayTasks(TaskTableView[] toDos, TaskTableView[] progress, TaskTableView[] done)
    {
        
    }

    private void DisplayEndLine()
    {
        Console.WriteLine("\n" + new string('-', TABLE_WIDTH));
    }
}