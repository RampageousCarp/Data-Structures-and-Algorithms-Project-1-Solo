using Project1.Models;
using Project1.Models.ViewModels;
using Project1.Repositories.Interfaces;
using Project1.Services.Interfaces;

namespace Project1.Services;

public class UserService : IUserService
{
    private readonly IGenericRepository<User> _repository;
    private readonly IMyCollection<User> _users;
    private readonly IMyCollectionFactory _collectionFactory;
    private int _lastId;
    
    public UserService(IGenericRepository<User> repository, IMyCollection<User> collection, IMyCollectionFactory collectionFactory)
    {
        _repository = repository;
        _users = collection;
        _collectionFactory = collectionFactory;
        _lastId = LoadLastId(_users.GetIterator());
    }

    public User[] GetAllUsers()
    {
        User[] allUsers = new User[_users.Count];
        IMyIterator<User> iterator = _users.GetIterator();
        iterator.Reset();

        int index = 0;
        while (iterator.HasNext())
            allUsers[index++] = iterator.Next();

        return allUsers;
    }

    public User? GetUserById(int id)
    {
        User? user = _users.FindBy<int>(id, (u, key) => u.Id.CompareTo(key));

        return user;
    }

    public void AddUser(CreateUserModel user)
    {
        User newUser = new User 
        {
            Id = ++_lastId,
            Username = user.Username!,
            FirstName = user.FirstName!,
            LastName = user.LastName!
        };
        
        _users.Add(newUser);
        _users.IncreaseDirty();

        SaveChanges();
    }

    public void RemoveUser(int id)
    {
        User? user = _users.FindBy(id, (t, key) => t.Id.CompareTo(key));
        if (user is null)
            return ;
        
        _users.Remove(user);
        _users.IncreaseDirty();

        SaveChanges();
    }

    private int LoadLastId(IMyIterator<User> items)
    {
        items.Reset();
        int lastId = 0;
        while (items.HasNext())
        {
            int currId = items.Next().Id;
            if (lastId < currId)
                lastId = currId;
        }

        return lastId;
    }

    public void SaveChanges()
    {
        if (_users.Dirty)
            _repository.SaveItems(_users.GetIterator(), _users.Count);
    }
}