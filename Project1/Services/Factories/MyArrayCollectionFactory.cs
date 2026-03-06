using Project1.Services.Collections;
using Project1.Services.Interfaces;

namespace Project1.Services.Factories;

public class MyArrayCollectionFactory : IMyCollectionFactory
{
    public IMyCollection<T> Create<T>() => new MyArrayCollection<T>();
}