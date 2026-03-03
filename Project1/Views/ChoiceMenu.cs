namespace Project1.Views;

public class ChoiceMenu<T>
{
    public int GetChoice(T?[] choices, bool clearScreen = false, string? title = null)
    {
        
        bool isSelected = false;
        int currentChoice = 0;
        Console.CursorVisible = false;
        
        PassEmptyChoices(choices, 1, currentChoice);
        while (!isSelected)
        {
            if (clearScreen)
                Console.Clear();
            if (title is not null)
                Console.Write(title);
            
            DisplayChoices(choices, currentChoice);
            int keyResult = ReadKey(choices, currentChoice);
            if (keyResult == -1)
                isSelected = true;
            else
                currentChoice = keyResult;
        }

        Console.CursorVisible = true;
        return currentChoice;
    }
    
    private void DisplayChoices(T?[] choices, int choiceIndex)
    {
        for (int i = 0; i < choices.Length; i++)
        {
            if (choiceIndex == i)
                Console.BackgroundColor = ConsoleColor.Gray;

            Console.Write(choices[i]?.ToString());
            Console.ResetColor();
            Console.WriteLine();
        }
        
        int newTop = Math.Max(0, Console.CursorTop - choices.Length);
        Console.SetCursorPosition(Console.CursorLeft, newTop);
        
    }
    
    private int ReadKey(T?[] choices, int currentChoice)
    {
        ConsoleKeyInfo key = Console.ReadKey(true);

        switch (key.Key)
        {
            case ConsoleKey.UpArrow:
                return MoveUp(choices, currentChoice);

            case ConsoleKey.DownArrow:
                return MoveDown(choices, currentChoice);

            case ConsoleKey.Enter:
                return -1;
            
            default:
                return -1;
        }
    }
    
    private int MoveUp(T?[] choices, int currentChoice)
    {
        currentChoice = (currentChoice == 0) ? choices.Length - 1 : currentChoice - 1;
        currentChoice = PassEmptyChoices(choices, -1, currentChoice);

        return currentChoice;
    }

    private int MoveDown(T?[] choices, int currentChoice)
    {
        currentChoice = (currentChoice == choices.Length - 1) ? 0 : currentChoice + 1;
        currentChoice = PassEmptyChoices(choices, 1, currentChoice);

        return currentChoice;
    }
    
    private int PassEmptyChoices(T?[] choices, int direction, int currentChoice)
    {
        if (currentChoice == choices.Length)
            return currentChoice;
        
        if (choices[currentChoice] is null)
        {
            if (direction > 0)
                currentChoice = MoveDown(choices, currentChoice);
            else
                currentChoice = MoveUp(choices, currentChoice);
        }

        return currentChoice;
    }
}