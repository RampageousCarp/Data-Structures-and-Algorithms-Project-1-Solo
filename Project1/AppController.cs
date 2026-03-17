using Project1.Models;
using Project1.Services.Interfaces;
using Project1.Views;

namespace Project1;

public class AppController
{
    private readonly Session _session;
    private readonly IUserService _userService;
    private readonly ITaskService _taskService;

    public AppController(Session session, IUserService userService,ITaskService taskService)
    {
        _session = session;
        _userService = userService;
        _taskService = taskService;
    }

    public void Run()
    {
        while (true)
        {
            if (!_session.IsLoggedIn)
            {
                bool shouldExit = RunUserSelectionView();
                if (shouldExit) return;
            }
            else
                RunTaskView();
        }
    }

    private bool RunUserSelectionView()
    {
        UserSelectionView userView = new UserSelectionView(_userService);
        User? selectedUser = userView.ChooseUser();
        
        if (selectedUser is null)
            return true;
        
        _session.Login(selectedUser);
        return false;
    }

    private void RunTaskView()
    {
        ConsoleTaskView taskView = new ConsoleTaskView(_taskService, _session);
        taskView.Run();
    }
}