using System.Collections;
using Project1.Services.Collections;
using Project1.Services.Interfaces;

namespace Project1.Services.Factories;

public class MyBSTCollectionSortableFactory : IMyCollectionFactory
{

    private readonly Hashtable _comparisons = new Hashtable();
    
    public MyBSTCollectionSortableFactory RegisterComparison<T>(Comparison<T> comparison)
    {
        _comparisons[typeof(T)] = comparison;
        return this;
    }
    
    public IMyCollection<T> Create<T>()
    {
        return new MyBSTCollectionSortable<T>(ResolveComparison<T>());
    }

    public IMyCollection<T> Create<T>(IMyIterator<T> iterator)
    {
        return new MyBSTCollectionSortable<T>(iterator, ResolveComparison<T>());
    }
    
    private Comparison<T> ResolveComparison<T>()
    {
        if (_comparisons.ContainsKey(typeof(T)))
        {
            object? raw = _comparisons[typeof(T)];
            if (raw is Comparison<T> comparison)
                return comparison;
        }

        return null!;
    }
}