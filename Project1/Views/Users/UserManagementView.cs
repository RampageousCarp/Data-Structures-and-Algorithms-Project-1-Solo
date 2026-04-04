using Project1.Models.ViewModels;
using Project1.Services.Interfaces;

namespace Project1.Views.Users;

public class UserManagementView
{
    private readonly ChoiceMenu _menu;
    private readonly IUserService _userService;
    private readonly Action<int> _onUserDeleted;

    public UserManagementView(IUserService userService, Action<int> onUserDeleted)
    {
        _menu = new ChoiceMenu();
        _userService = userService;
        _onUserDeleted = onUserDeleted;
    }

    public void Run()
    {
        while (true)
        {
            Console.Clear();
            string?[] options = ["Add User", "Delete User", null, "Exit"];
            Console.WriteLine($"=== Manage Users ===\n");

            int choice = _menu.GetChoice(options);
            switch (choice)
            {
                case 0:
                    AddUser();
                    break;
                case 1:
                    DeleteUser();
                    break;
                default:
                    return;
            }
        }
    }

    private void AddUser()
    {
        CreateUserModel newUser = new CreateUserModel();
        bool dataIncomplete = false;

        while (true)
        {
            string?[] fieldsToEnter =
            [
                $"Username: {newUser.Username}",
                $"First Name: {newUser.FirstName}",
                $"Last Name: {newUser.LastName}",
                null,
                "Add",
                "Exit"
            ];
            
            Console.Clear();
            Console.WriteLine("=== Add New User ===\n");
            if (dataIncomplete)
                Console.WriteLine("Not all fields are filled in!\n");
            dataIncomplete = false;

            
            int option = _menu.GetChoice(fieldsToEnter);

            switch (option)
            {
                case 0:
                    newUser.Username = EnterField("Username");
                    break;
                case 1:
                    newUser.FirstName = EnterField("First Name");
                    break;
                case 2:
                    newUser.LastName = EnterField("Last Name");
                    break;
                case 4:
                    if (!IsValid(newUser))
                        dataIncomplete = true;
                    else
                    {
                        _userService.AddUser(newUser);
                        return;
                    }
                    break;
                default:
                    return;
            }
        }
    }
    

    private string EnterField(string fieldName)
    {
        Console.Clear();
        Console.WriteLine($"=== Enter {fieldName} ===\n");
        Console.Write($"{fieldName}: ");
        
        string? field = Console.ReadLine();

        return field ?? "";
    }

    private bool IsValid(CreateUserModel newUser)
    {
        return !string.IsNullOrEmpty(newUser.Username) || !string.IsNullOrEmpty(newUser.FirstName) ||
               !string.IsNullOrEmpty(newUser.LastName);
    }

    private void DeleteUser()
    {
        
    }
}