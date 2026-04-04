using Project1.Models;
using Project1.Models.ENums;
using Project1.Services.Interfaces;
using Project1.Views.Users;

namespace Project1.Views;

public class StartMenuView
{
    private readonly ChoiceMenu _menu;
    private readonly UserSelectionView _userSelectionView;
    private readonly UserManagementView _userManagementView;
    private readonly Session _session;

    public StartMenuView(IUserService userService, ITaskService taskService, Session session)
    {
        _menu = new ChoiceMenu();
        _session = session;
        _userSelectionView = new UserSelectionView(userService);
        _userManagementView = new UserManagementView(userService, userId =>
        {
            taskService.UnassignUser(userId);  // clean up tasks first
            userService.RemoveUser(userId);     // then remove the user
        });
    }

    public StartMenuResult Run()
    {
        while (true)
        {
            Console.Clear();
            Console.WriteLine($"=== KANBAN BOARD APP ===\n");

            string?[] options = ["Login", "Manage Users", null, "Exit"];
            int choice = _menu.GetChoice(options);
            
            switch (choice)
            {
                case 0:
                    User? selectedUser = _userSelectionView.ChooseUser();
                    if (selectedUser is not null)
                    {
                        _session.Login(selectedUser);
                        return StartMenuResult.LoggedIn;
                    }
                    break;

                case 1:
                    _userManagementView.Run();
                    break;

                default:
                    return StartMenuResult.Exit;
            }
        }
    }
}