using Project1.Models.ViewModels;

namespace Project1.Views;

public class FiltersMenu
{
    private readonly ChoiceMenu<string> _menu;
    private TaskFilter _filters;
    
    public FiltersMenu(ChoiceMenu<string> menu, TaskFilter filters)
    {
        _menu = menu;
        _filters = filters;
    }

    public void SelectFilters()
    {
        while (true)
        {
            int parameter = ChooseFilterParameter();
            
            switch (parameter)
            {
                case 0:
                    
                case 1:
                    
                    break;
                case 2:
                    _filters.ResetFilters();
                    break;
                case 3:
                    break;
                
                default:
                    return;
            }
        }
    }

    private int ChooseFilterParameter()
    {
        string?[] parameters =
        [
            $"Filter by Priority: {(_filters.Priority is null ? "---" : _filters.Priority)}",
            $"Filter by Due Date: {GetDueToFilterString()}",
            $"Filter by Creation Date: {GetCreationDateFilterString()}",
            $"Search by keyword: {_filters.Keyword ?? "---"}",
            $"Sort by: {(_filters.ApplySort ? _filters.SortBy : "---" )}",
            $"Sort order: {(_filters.ApplySort ? _filters.SortOrder : "---" )}",
            null,
            "Reset filters",
            "Exit",
            
        ];
        
        Console.Clear();
        Console.WriteLine("=== Apply Filters ===\n");

        return _menu.GetChoice(parameters);
    }

    private string GetDueToFilterString()
    {
        string filterString = "";

        if (_filters.DueToFrom is not null)
            filterString += $"From: {_filters.DueToFrom:dd-MM-yyyy} ";
        
        if(_filters.DueToTo is not null)
            filterString += $"To: {_filters.DueToTo:dd-MM-yyyy}";

        return string.IsNullOrWhiteSpace(filterString) ? "---" : filterString;
    }
    
    private string GetCreationDateFilterString()
    {
        string filterString = "";

        if (_filters.CreatedAtFrom is not null)
            filterString += $"From: {_filters.CreatedAtFrom:dd-MM-yyyy} ";
        
        if(_filters.CreatedAtTo is not null)
            filterString += $"To: {_filters.CreatedAtTo:dd-MM-yyyy}";

        return string.IsNullOrWhiteSpace(filterString) ? "---" : filterString;
    }
}