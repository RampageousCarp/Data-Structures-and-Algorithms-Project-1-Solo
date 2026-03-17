using Project1.Services.Interfaces;

namespace Project1.Services.Collections;

public class MyArrayCollection<T> : IMyCollection<T>
{
    protected T[] _items;
    protected int _count;
    protected bool _dirty = false;
    private const long DEFAULT_CAPACITY = 64;
    
    public MyArrayCollection()
    {
        _items = new T[DEFAULT_CAPACITY];
        _count = 0;
        _dirty = false;
    }

    public MyArrayCollection(IMyIterator<T> iterator)
    {
        _dirty = false;
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
                if (_count == _items.Length)
                    Array.Resize(ref _items,_items.Length * 2);

                _items[_count++] = iterator.Next();
            }
        }
    }

    #region Methods

    public IMyCollection<T> Create<T>() => new MyArrayCollection<T>();

    public void Add(T item)
    {
        SetDirty();
        if (_count + 1 >= _items.Length)
            Array.Resize(ref _items,_items.Length * 2);

        _items[_count++] = item;
    }

    public void Remove(T item)
    {
        int posToDelete = Find(item);
        
        if (posToDelete != -1)
        {
            if (posToDelete < _count) 
                Shift(posToDelete, false);
            _items[-- _count] = default!;
        }
    }

    public T? FindBy<K>(K key, Func<T, K, bool> comparer)
    {
        for (int i = 0; i < _count; i ++)
        
            if (comparer(_items[i], key))
                return _items[i];

        return default(T);
    }

    public IMyCollection<T> Filter(Func<T, bool> predicate)
    {
        MyArrayCollection<T> result = new MyArrayCollection<T>();
        
        for (int i = 0; i < _count; i++)
            if (predicate(_items[i]))
                result.Add(_items[i]);

        return result;
    }

    public void Sort(Comparison<T> comparison)
    {
        if(_count > 1)
            QuickSort(0, _count -1, comparison);
    }
    
    public int Count => _count;

    public bool Dirty
    {
        get => _dirty;
        set => _dirty = value;
    }
    
    public R Reduce<R>(Func<R, T, R> accumulator)
    {
        R acc = default(R);

        for (int i = 0; i < _count; i++)
            acc = accumulator(acc, _items[i]);

        return acc;
    }

    public R Reduce<R>(R initial, Func<R, T, R> accumulator)
    {
        R acc = initial;

        for (int i = 0; i < _count; i++)
            acc = accumulator(acc, _items[i]);

        return acc;
    }

    public IMyIterator<T> GetIterator()
    {
        return new ArrayCollectionIterator<T>(this);
    }
    #endregion

    #region Helpers
    private int Find(T item, int startIndex = 0)
    {
        int itemPos = -1;
        if (startIndex > _count)
            return -1;

        for (int i = startIndex; i < _count; i++)
            if (_items[i]!.Equals(item))
            {
                itemPos = i;
                break;
            }

        return itemPos;
    }

    private void SetDirty() => _dirty = true;
    
    private void Shift(int i, bool right = true)
    {
        if (right)
            for (int index = _count + 1; index >= i; index--)
                _items[index] = _items[index - 1];
        
        else
            for (int index = i; index < _count; index ++)
                _items[index] = _items[index + 1];
    }

    private void QuickSort(int low, int high, Comparison<T> comparison)
    {
        if (low < high)
        {
            int pivotPosition = Partition(low, high, comparison);
            QuickSort(low, pivotPosition - 1, comparison);
            QuickSort(pivotPosition + 1, high, comparison);
        }
    }

    private int Partition(int low, int high, Comparison<T> comparison)
    {
        T pivot = _items[high];
        int i = low - 1;
        for (int j = low; j <= high - 1; j++)
            if (comparison(_items[j], pivot) < 0)
            {
                i++;
                (_items[i], _items[j]) = (_items[j], _items[i]);
            }

        (_items[i + 1], _items[high]) = (_items[high], _items[i + 1]);
        return i + 1;
    }
    #endregion

    #region InnerClass

    private class ArrayCollectionIterator<T> : IMyIterator<T>
    {
        private readonly MyArrayCollection<T> _collection;
        private int _currentIndex;
        
        public ArrayCollectionIterator(MyArrayCollection<T> collection)
        {
            _collection = collection;
            _currentIndex = -1;
        }
        
        public bool HasNext()
        {
            return _currentIndex + 1 < _collection._count;
        }
        
        public T Next()
        {
            if (!HasNext())
                return default(T);

            return _collection._items[++_currentIndex];
        }
        
        public void Reset()
        {
            _currentIndex = -1;
        }
    }
    #endregion
}