using Project1.Services.Interfaces;

namespace Project1.Services.Collections;

public class MyBSTCollectionSortable<T> : MyBSTCollection<T>
{
    private T[] _sortedSnapshot;
    private bool _isSorted;
    private Comparison<T>? _sortComparison;
    
    public MyBSTCollectionSortable(Comparison<T> defaultComparison)
        : base(defaultComparison) { }

    public MyBSTCollectionSortable(IMyIterator<T> iterator, Comparison<T> defaultComparison)
        : base(iterator, defaultComparison) { }

    #region Overrides

    

    #endregion

    #region Helpers

    

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