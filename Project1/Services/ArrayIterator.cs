using Project1.Services.Interfaces;

namespace Project1.Services;

public class ArrayIterator<T> : IMyIterator<T>
{
    private readonly T[] _items;
    private readonly int _count;
    private int _currentIndex;

    public ArrayIterator(T[] items, int count)
    {
        _items = items;
        _count = count;
        _currentIndex = -1;
    }
    
    public bool HasNext()
    {
        return _currentIndex + 1 < _count;
    }

    public T Next()
    {
        if (!HasNext())
            return default(T);

        return _items[++_currentIndex];
    }

    public void Reset()
    {
        _currentIndex = -1;
    }

    public int GetCount()
    {
        return _count;
    }
}