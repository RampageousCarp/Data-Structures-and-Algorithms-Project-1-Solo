using Project1.Services.Collections;
using Project1.Services.Interfaces;

namespace Project1.Services.Factories;

public class MyHashMapCollectionSortableFactory : IMyCollectionFactory
{
    public IMyCollection<T> Create<T>() => new MyHashMapCollectionSortable<T>();
    public IMyCollection<T> Create<T>(IMyIterator<T> iterator) => new MyHashMapCollectionSortable<T>(iterator);
}