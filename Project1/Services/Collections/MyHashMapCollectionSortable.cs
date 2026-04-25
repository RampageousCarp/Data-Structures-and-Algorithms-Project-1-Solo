using Project1.Services.Interfaces;

namespace Project1.Services.Collections;

public class MyHashMapCollectionSortable<T> : MyHashMapCollection<T>
{
    private T[]? _sortedSnapshot;
    private bool _isSorted;
    
    public MyHashMapCollectionSortable() : base() { }

    public MyHashMapCollectionSortable(IMyIterator<T> iterator) : base(iterator) { }

    #region Overrides

    public override void Add(T item)
    {
        base.Add(item);
        InvalidateSnapshot();
    }

    public override void Remove(T item)
    {
        base.Remove(item);
        InvalidateSnapshot();
    }

    public override IMyCollection<T> Filter(Func<T, bool> predicate)
    {
        IMyCollection<T> filtered = new MyHashMapCollectionSortable<T>();

        for (int i = 0; i < _buckets.Length; i++)
        {
            Node? current = _buckets[i];
            while (current != null)
            {
                if(predicate(current.Data))
                    filtered.Add(current.Data);

                current = current.Next;
            }
        }

        return filtered;
    }

    public override void Sort(Comparison<T>? comparison)
    {
        if (comparison == null)
        {
            _isSorted = false;
            return;
        }
            
        if (_isSorted) return;
            
            
            
        T[] items = ToArray();

        QuickSort(items, 0, _count - 1, comparison);

        _sortedSnapshot = items;
        _isSorted = true;
    }

    public override IMyIterator<T> GetIterator()
    {
        return _isSorted && _sortedSnapshot != null
            ? new SortedSnapshotIterator(_sortedSnapshot)
            : base.GetIterator();
    }

    #endregion

    #region Helpers

    private void InvalidateSnapshot()
    {
        _sortedSnapshot = null;
        _isSorted = false;
    }
    
    private T[] ToArray()
    {
        T[] arrayItems = new T[_count];

        IMyIterator<T> iterator = base.GetIterator();
        iterator.Reset();
        
        int p = 0;
        while (iterator.HasNext())
            arrayItems[p++] = iterator.Next();

        return arrayItems;
    }
    
    private void QuickSort(T[] items, int low, int high, Comparison<T> comparison)
    {
        if (low < high)
        {
            int pivotPosition = Partition(items, low, high, comparison);
            QuickSort(items, low, pivotPosition - 1, comparison);
            QuickSort(items,pivotPosition + 1, high, comparison);
        }
    }

    private int Partition(T[] items, int low, int high, Comparison<T> comparison)
    {
        T pivot = items[high];
        int i = low - 1;
        for (int j = low; j <= high - 1; j++)
            if (comparison(items[j], pivot) < 0)
            {
                i++;
                (items[i], items[j]) = (items[j], items[i]);
            }

        (items[i + 1], items[high]) = (items[high], items[i + 1]);
        return i + 1;
    }

    #endregion

    #region InnerClasses
    
    private class SortedSnapshotIterator : IMyIterator<T>
    {

        private readonly T[] _snapshot;
        private int _index = -1;
        
        public SortedSnapshotIterator(T[] snapshot)
        {
            _snapshot = snapshot;
        }

        public bool HasNext() => _index + 1 < _snapshot.Length;

        public T Next() => _snapshot[++_index];

        public void Reset() => _index = -1;
    }

    #endregion
}