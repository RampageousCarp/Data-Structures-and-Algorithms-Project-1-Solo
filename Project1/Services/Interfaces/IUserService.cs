using Project1.Models;

namespace Project1.Services.Interfaces;

public interface IUserService
{
    public User[] GetAllUsers();
}