using Project1.Models;
using Project1.Services.Interfaces;

namespace Project1.Views;

public class UserSelectionView
{
    private ChoiceMenu _menu;
    private readonly IUserService _userService;

    public UserSelectionView(IUserService userService)
    {
        _menu = new ChoiceMenu();
        _userService = userService;
    }
    
    public User? ChooseUser()
    {
        User[] users = _userService.GetAllUsers();
        string[] itemsToDisplay = new String[users.Length + 1];
        
        for (int i = 0; i < users.Length; i++)
            itemsToDisplay[i] = users[i].ToMenuString();

        itemsToDisplay[^1] = "Exit";

        while (true)
        {
            Console.Clear();

            int chosenIndex = _menu.GetChoice(itemsToDisplay, true, "=== Choose User ===\n\n");
            if (chosenIndex == itemsToDisplay.Length - 1)
                return null;
            else
                return users[chosenIndex];
        }
    }
}