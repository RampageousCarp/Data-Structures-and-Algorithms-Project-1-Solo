using Project1.Services.Collections;
using Project1.Services.Interfaces;

namespace Project1.Services.Factories;

public class MyLinkedListCollectionFactory : IMyCollectionFactory
{
    public IMyCollection<T> Create<T>() => new MyLinkedListCollection<T>();
    public IMyCollection<T> Create<T>(IMyIterator<T> iterator) => new MyLinkedListCollection<T>(iterator);
    
}