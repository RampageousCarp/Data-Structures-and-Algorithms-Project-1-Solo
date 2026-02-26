using Project1.Services.Interfaces;

namespace Project1.Services.Collections;

public class MyArrayCollection<T> : IMyCollection<T>
{
    private T[] _items;
    private long _count;
    private const long DEFAULT_CAPACITY = 256;
    
    public MyArrayCollection()
    {
        _items = new T[DEFAULT_CAPACITY];
        _count = 0;
    }

    public MyArrayCollection(IMyIterator<T> iterator)
    {
        if (iterator is ArrayIterator<T> arrayIterator)
        {
            long itemsCount = arrayIterator.GetCount();
            _items = new T[itemsCount * 2];
            _count = 0;

            while (arrayIterator.HasNext())
                _items[_count++] = arrayIterator.Next();
        }
        else
        {
            _items = new T[DEFAULT_CAPACITY];
            _count = 0;

            while (iterator.HasNext())
            {
                if (_count == _items.LongLength)
                    Array.Resize(ref _items,_items.Length * 2);

                _items[_count++] = iterator.Next();
            }
        }
    }
    public void Add(T item)
    {
        throw new NotImplementedException();
    }

    public void Remove(T item)
    {
        throw new NotImplementedException();
    }

    public T FindBy<K>(K key, Func<T, K, bool> comparer)
    {
        throw new NotImplementedException();
    }

    public IMyCollection<T> Filter(Func<T, bool> predicate)
    {
        throw new NotImplementedException();
    }

    public void Sort(Comparison<T> comparison)
    {
        throw new NotImplementedException();
    }

    public int Count { get; }
    public bool Dirty { get; set; }
    public R Reduce<R>(Func<R, T, R> accumulator)
    {
        throw new NotImplementedException();
    }

    public R Reduce<R>(R initial, Func<R, T, R> accumulator)
    {
        throw new NotImplementedException();
    }

    public IMyIterator<T> GetIterator()
    {
        throw new NotImplementedException();
    }

    public IEnumerator<T> GetEnumerator()
    {
        throw new NotImplementedException();
    }
}