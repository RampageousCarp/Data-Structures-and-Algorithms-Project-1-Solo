namespace Project1.Models.ViewModels;

public class MenuOption<T>
{
    private readonly string _label;
    public T? Value { get; }
    public bool IsAction { get; }

    public MenuOption(T value, string? label = null)
    {
        Value = value;
        _label = label ?? value?.ToString() ?? string.Empty;
        IsAction = false;
    }

    public MenuOption(string label)
    {
        Value = default;
        _label = label;
        IsAction = true;
    }

    public override string ToString() => _label;
}