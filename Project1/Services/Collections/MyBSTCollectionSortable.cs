using Project1.Services.Interfaces;

namespace Project1.Services.Collections;

public class MyBSTCollectionSortable<T> : MyBSTCollection<T>
{
    private T[]? _sortedSnapshot;
    private bool _isSorted;
    private Comparison<T>? _sortComparison;
    
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