namespace Project1.Views;

public class ChoiceMenu<T>
{
    private ConsoleKeyInfo _key;
    private int _currentChoice;
    private bool _isSelected;

    public int GetChoice(T?[] choices)
    {
        _isSelected = false;
        _currentChoice = 0;
        Console.CursorVisible = false;
        PassEmptyChoices(choices, 1);
        while (!_isSelected)
        {
            DisplayChoices(choices, _currentChoice);
            ReadKey(choices);
        }

        Console.CursorVisible = true;
        return _currentChoice;
    }
    
    private void DisplayChoices(T?[] choices, int choiceIndex)
    {
        for (int i = 0; i < choices.Length; i++)
        {
            if (choiceIndex == i)
                Console.BackgroundColor = ConsoleColor.Gray;

            Console.WriteLine(choices[i]?.ToString());
            Console.ResetColor();
        }
        
        int newTop = Math.Max(0, Console.CursorTop - choices.Length);
        Console.SetCursorPosition(Console.CursorLeft, newTop);
    }
    
    private void ReadKey(T?[] choices)
    {
        _key = Console.ReadKey(true);

        switch (_key.Key)
        {
            case ConsoleKey.UpArrow:
                MoveUp(choices);
                break;

            case ConsoleKey.DownArrow:
                MoveDown(choices);
                break;

            case ConsoleKey.Enter:
                _isSelected = true;
                break;
        }
    }
    
    private void MoveUp(T?[] choices)
    {
        _currentChoice = (_currentChoice == 0) ? choices.Length - 1 : _currentChoice - 1;
        PassEmptyChoices(choices, -1);
    }

    private void MoveDown(T?[] choices)
    {
        _currentChoice = (_currentChoice == choices.Length - 1) ? 0 : _currentChoice + 1;
        PassEmptyChoices(choices, 1);
    }
    
    private void PassEmptyChoices(T?[] choices, int direction)
    {
        if (_currentChoice == choices.Length)
            return;
        
        if (choices[_currentChoice] is null)
        {
            if (direction > 0)
                MoveDown(choices);
            else
                MoveUp(choices);
        }
    }
}