using Project1.Services.Interfaces;

namespace Project1.Services.Collections;

public class MyBSTCollectionSortable<T> : MyBSTCollection<T>
{
    private T[]? _sortedSnapshot;
    private bool _isSorted;
    
    public MyBSTCollectionSortable(Comparison<T> defaultComparison)
        : base(defaultComparison) { }

    public MyBSTCollectionSortable(IMyIterator<T> iterator, Comparison<T> defaultComparison)
        : base(iterator, defaultComparison) { }

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

    public override void Sort(Comparison<T>? comparison)
    {
        if (comparison is null)
        {
            InvalidateSnapshot();
            return;
        }
        
        if (_isSorted)
            return;

        T[] items = ToArray();
        QuickSort(items, 0, _count - 1, comparison);
        
        _isSorted = true;
        _sortedSnapshot = items;

    }

    public override IMyCollection<T> Filter(Func<T, bool> predicate)
    {
        MyBSTCollectionSortable<T> filtered = new MyBSTCollectionSortable<T>(_defaultComparison);
        
        FilterInOrder(_root, predicate, filtered);
        return filtered;
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
        T[] arr = new T[_count];
        FillInOrder(_root, arr, 0);
        
        return arr;
    }

    private int FillInOrder(Node? node, T[] arr, int index)
    {
        if (node == null)
            return index;
        
        index = FillInOrder(node.Left, arr, index);
        arr[index++] = node.Data;
        return FillInOrder(node.Right, arr, index);
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