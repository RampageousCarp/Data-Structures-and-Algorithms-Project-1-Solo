using Project1.Models;
using Project1.Models.ViewModels;

namespace Project1.Services.Interfaces;

public interface IUserService
{
    public User[] GetAllUsers();
    public User? GetUserById(int id);
    User AddUser(CreateUserModel user);
    bool RemoveUser(int id);
}