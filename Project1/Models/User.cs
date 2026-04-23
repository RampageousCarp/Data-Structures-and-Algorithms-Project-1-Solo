namespace Project1.Models;

public class User
{
    public int Id { get; set; }
    public string Username { get; set; } = null!;
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;

    public string ToMenuString()
    {
        return Username + "\n";
    }

    public override int GetHashCode() => Id.GetHashCode();
}