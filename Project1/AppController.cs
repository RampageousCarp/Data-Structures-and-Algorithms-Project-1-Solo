using Project1.Models;
using Project1.Models.ENums;
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
                StartMenuView startMenu = new StartMenuView(_userService, _taskService, _session);
                StartMenuResult result = startMenu.Run();
                if (result == StartMenuResult.Exit)
                    return;
            }
            else
                RunTaskView();
        }
    }

    private void RunTaskView()
    {
        ConsoleTaskView taskView = new ConsoleTaskView(_taskService, _userService, _session);
        taskView.Run();
    }
}