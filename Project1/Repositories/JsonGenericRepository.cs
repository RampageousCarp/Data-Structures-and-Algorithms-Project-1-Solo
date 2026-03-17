using System.Text.Json;
using Project1.Repositories.Interfaces;
using Project1.Services.Interfaces;

namespace Project1.Repositories;

class JsonGenericRepository<T> : IGenericRepository<T>
{
    private readonly string _filePath;
    public JsonGenericRepository(string filePath) => _filePath = filePath;

    public T[] LoadItems()
    {
        if (!File.Exists(_filePath))
            return new T[0];

        string json = File.ReadAllText(_filePath);

        var itemsList = JsonSerializer.Deserialize<List<T>>(json);

        if (itemsList == null || itemsList.Count == 0)
            return new T[0];

        T[] items = new T[itemsList.Count];
        for (int i = 0; i < itemsList.Count; i++)
            items[i] = itemsList[i];

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

        File.WriteAllText(_filePath, json);
    }
}