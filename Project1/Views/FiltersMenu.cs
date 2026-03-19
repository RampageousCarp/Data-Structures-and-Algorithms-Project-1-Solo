using Project1.Models.ENums;
using Project1.Models.ViewModels;

namespace Project1.Views;

public class FiltersMenu
{
    private readonly ChoiceMenu<string> _menu;
    private TaskFilter _filters;
    
    public FiltersMenu(TaskFilter filters)
    {
        _menu = new ChoiceMenu<string>();
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
                    EnterPriority();
                    break;
                case 1:
                    (DateOnly? from, DateOnly? to)? dueToDateFilter = EnterDateFilter("Due To");
                    
                    if (dueToDateFilter is null)
                       break;
                    
                    _filters.DueToFrom = dueToDateFilter.Value.from;
                    _filters.DueToTo = dueToDateFilter.Value.to;
                    
                    break;
                case 2:
                    (DateOnly? from, DateOnly? to)? creationDateFilter = EnterDateFilter("Creation");
                    
                    if (creationDateFilter is null)
                        break;
                   
                    _filters.CreatedAtFrom = creationDateFilter.Value.from;
                    _filters.CreatedAtTo = creationDateFilter.Value.to;
                    
                    break;
                case 3:
                    EnterKeyword();
                    break;
                case 4:
                    EnterSortParameter();
                    break;
                case 5:
                    EnterSortOrder();
                    break;
                case 7:
                    _filters.ResetFilters();
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
            $"Filter by Due To Date: {GetDueToFilterString()}",
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
    
    private void EnterPriority()
    {
        Console.Clear();
        Console.WriteLine("=== Filter By Priority ===\n");

        string[] priorities = Enum.GetNames(typeof(TaskPriority));
        int selectedIndex = _menu.GetChoice(priorities);

        _filters.Priority = (TaskPriority)selectedIndex;
    }
    
    private void EnterKeyword()
    {
        Console.Clear();
        Console.WriteLine("=== Search By Keyword ===\n");
        Console.Write("Keyword: ");

        string? keyword = Console.ReadLine();

        _filters.Keyword = string.IsNullOrWhiteSpace(keyword) ? null : keyword;
    }

    private void EnterSortParameter()
    {
        Console.Clear();
        Console.WriteLine("=== Sort By ===\n");

        string[] sortValues= Enum.GetNames(typeof(SortingValue));
        int selectedIndex = _menu.GetChoice(sortValues);

        _filters.SortBy = (SortingValue)selectedIndex;
    }
    
    private void EnterSortOrder()
    {
        Console.Clear();
        Console.WriteLine("=== Sort Order ===\n");

        string[] sortOrders= Enum.GetNames(typeof(SortOrder));
        int selectedIndex = _menu.GetChoice(sortOrders);

        _filters.SortOrder = (SortOrder)selectedIndex;
    }

    private (DateOnly? from, DateOnly? to)? EnterDateFilter(string dateType)
    {
        Console.Clear();
        Console.WriteLine($"=== Filter By {dateType} Date ===\n");

        string?[] options =
        [
            "Today",
            "This week",
            "This month",
            "Custom range",
            null,
            "Clear date filter",
            "Exit"
        ];

        int option = _menu.GetChoice(options);
        
        switch (option)
        {
            case 0:
                return (DateOnly.FromDateTime(DateTime.Today), DateOnly.FromDateTime(DateTime.Today));
            case 1:
                return GetWeekStartAndEnd();
            case 2:
                return GetMonthStartAndEnd();
            case 3:
                return EnterCustomDate();
            case 5:
                return (null, null);
                
            default:
                return null;
        }

    }

    private (DateOnly from, DateOnly to) GetWeekStartAndEnd()
    {
        DateOnly today = DateOnly.FromDateTime(DateTime.Today);

        int diff = (7 + (today.DayOfWeek - DayOfWeek.Monday)) % 7;
        DateOnly from = today.AddDays(-diff);
        DateOnly to = from.AddDays(6);

        return (from, to);
    }
    
    private (DateOnly from, DateOnly to) GetMonthStartAndEnd()
    {
        DateOnly today = DateOnly.FromDateTime(DateTime.Today);

        DateOnly from = new DateOnly(today.Year, today.Month, 1);
        DateOnly to = from.AddMonths(1).AddDays(-1);

        return (from, to);
    }

    private (DateOnly? from, DateOnly? to)? EnterCustomDate()
    {
        DateOnly? from = null;
        DateOnly? to = null;
        
        Console.Clear();
        Console.WriteLine("=== Enter From And To Date ===\n");

        while (true)
        {
            string?[] options =
            [
                $"From (dd-mm-yyyy): {(from is null ? "" : $"{from:dd-MM-yyyy}")}",
                $"To (dd-mm-yyyy): {(to is null ? "" : $"{to:dd-MM-yyyy}")}",
                null,
                "Enter",
                "Exit"
            ];
            
            int option = _menu.GetChoice(options, true, "=== Enter From And To Date ===\n\n");

            switch (option)
            {
                case 0:
                    from = EnterDate("From");
                    break;
                case 1:
                    to = EnterDate("To");
                    break;
                case 3:
                    return (from, to);
                default:
                    return null;
            }
        }
    }

    private DateOnly EnterDate(string dateType)
    {
        Console.Clear();
        Console.WriteLine($"=== Enter {dateType} Date ===\n");
        Console.Write("From (dd-mm-yyyy): ");

        string? dateString = Console.ReadLine();

        if (DateOnly.TryParseExact(dateString, "dd-MM-yyyy", out DateOnly date))
            return date;
        
        return DateOnly.FromDateTime(DateTime.Now);
    }
}