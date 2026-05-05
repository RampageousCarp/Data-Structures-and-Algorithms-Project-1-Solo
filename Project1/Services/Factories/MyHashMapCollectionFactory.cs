using Project1.Services.Collections;
using Project1.Services.Interfaces;

namespace Project1.Services.Factories;

public class MyHashMapCollectionFactory : IMyCollectionFactory
{
    public IMyCollection<T> Create<T>() => new MyHashMapCollection<T>();
    public IMyCollection<T> Create<T>(IMyIterator<T> iterator) => new MyHashMapCollection<T>(iterator);
}