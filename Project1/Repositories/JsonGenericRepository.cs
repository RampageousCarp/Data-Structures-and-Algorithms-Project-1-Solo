using System.Text.Json;
using Project1.Repositories.Interfaces;
using Project1.Services.Interfaces;

namespace Project1.Repositories;

public class JsonGenericRepository<T> : IGenericRepository<T>
{
    private readonly string _filePath;
    public JsonGenericRepository(string filePath) => _filePath = filePath;

    public T[] LoadItems()
    {
        if (!File.Exists(_filePath))
            return new T[0];

        string json = File.ReadAllText(_filePath);

        T[]? items = JsonSerializer.Deserialize<T[]>(json);

        return items;
    }

    public void SaveItems(IMyIterator<T> items, int count)
    {
        T[] itemsArray = new T[count];

        items.Reset();
        int pos = -1;
        while (items.HasNext())
            itemsArray[++pos] = items.Next();

        string json = JsonSerializer.Serialize(itemsArray, new JsonSerializerOptions
        {
            WriteIndented = true
        });
        
        string? directory = Path.GetDirectoryName(_filePath);
        if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }

        File.WriteAllText(_filePath, json);
    }
}