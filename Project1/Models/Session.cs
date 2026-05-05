namespace Project1.Models;

public class Session
{
    public User? CurrentUser { get; private set; }
    
    public bool IsLoggedIn => CurrentUser is not null;
    
    public void Login(User user) => CurrentUser = user;
    
    public void Logout() => CurrentUser = null;
}