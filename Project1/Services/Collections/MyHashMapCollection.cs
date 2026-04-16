using Project1.Services.Interfaces;

namespace Project1.Services.Collections;

public class MyHashMapCollection<T> : IMyCollection<T>
{
    
    private Node?[] _buckets;
    private int _count;
    private bool _dirty;
    private int _dirtyCount;
    private bool _isSorted = false;
    
    private T[]? _sortedSnapshot;
    private Comparison<T>? _sortComparison;

    private const int DEFAULT_CAPACITY = 16;
    private const double LOAD_FACTOR = 0.75;
    
    public MyHashMapCollection()
    {
        _buckets = new Node[DEFAULT_CAPACITY];
    }

    public MyHashMapCollection(IMyIterator<T> iterator)
    {
        _buckets = new Node[DEFAULT_CAPACITY];
        while (iterator.HasNext())
            Add(iterator.Next());

        _dirty = false;
    }

    #region Methodes
    
        public void Add(T item)
        {
            ResizeIfNeeded();
            InsertNode(item);
            
            IncreaseDirty();
            InvalidateSnapshot();
        }

        public void Remove(T item)
        {
            throw new NotImplementedException();
        }

        public T? FindBy<K>(K key, Func<T, K, int> comparer)
        {
            throw new NotImplementedException();
        }

        public IMyCollection<T> Filter(Func<T, bool> predicate)
        {
            throw new NotImplementedException();
        }

        public void Sort(Comparison<T>? comparison)
        {
            if (comparison == null)
            {
                _isSorted = false;
                return;
            }
            throw new NotImplementedException();
        }

        public int Count { get; }
        public bool Dirty { get; }
        public void IncreaseDirty()
        {
            _dirty = true;
            _dirtyCount++;
        }

        public void ResetDirty()
        {
            _dirty = false;
            _dirtyCount = 0;
        }

        public int GetDirtyCount() => _dirtyCount;

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
            return _isSorted && _sortedSnapshot != null
                ? new SortedSnapshotIterator(_sortedSnapshot)
                : new MyHashMapCollectionIterator(this);
        }
    
    #endregion

    #region Helpers


    private int GetBucketIndex(T item)
    {
        int hash = item!.GetHashCode();

        return Math.Abs(hash) % _buckets.Length;
    }
    
    private void ResizeIfNeeded()
    {
        if (_count < _buckets.Length * LOAD_FACTOR)
            return;

        Node?[] oldBuckets = _buckets;

        _buckets = new Node?[oldBuckets.Length * 2];

        for (int i = 0; i < oldBuckets.Length; i++)
        {
            Node? current = oldBuckets[i];
            while (current != null)
            {
                InsertNode(current.Data);
                current = current.Next;
            }
        }
    }

    private void InsertNode(T? item)
    {
        int index = GetBucketIndex(item!);

        Node? current = _buckets[index];

        if (current == null)
        {
            _buckets[index] = new Node(item!);
            return;
        }

        while (current.Next != null)
        {
            if (current.Data!.Equals(index))
                return;

            current = current.Next;
        }

        if (!current.Data!.Equals(item))
        {
            current.Next = new Node(item!);
            _count++;
        }
    }

    private void InvalidateSnapshot()
    {
        _sortedSnapshot = null;
        _isSorted = false;
        _sortComparison = null;
    }

    #endregion
    
    
    #region InnerClasses
    
    private class Node(T data)
    {
        public T Data = data;
        public Node? Next = null;
    }
    
    private class MyHashMapCollectionIterator : IMyIterator<T>
    {
        private readonly MyHashMapCollection<T> _collection;
        private int _bucketIndex;
        private Node? _current;
        private bool _started;
        
        public MyHashMapCollectionIterator(MyHashMapCollection<T> collection)
        {
            _collection = collection;
        }
        
        public bool HasNext()
        {
            if (!_started)
                return FindNextNode() != null;

            if (_current?.Next != null)
                return true;
            
            int savedIndex = _bucketIndex + 1;
            int temp = savedIndex;
            while (temp < _collection._buckets.Length)
            {
                if (_collection._buckets[temp] != null)
                    return true;
                temp++;
            }
            
            return false;
        }
        
        public T Next()
        {
            if (!_started)
            {
                _started = true;
                _bucketIndex = 0;
                _current = FindNextNode();
                _bucketIndex++;
                return _current!.Data;
            }

            if (_current?.Next != null)
            {
                _current = _current.Next;
                return _current!.Data;
            }

            while (_bucketIndex < _collection._buckets.Length)
            {
                if (_collection._buckets[_bucketIndex] != null)
                {
                    _current = _collection._buckets[_bucketIndex];
                    _bucketIndex++;
                    return _current!.Data;
                }
                _bucketIndex++;
            }

            return default!;
        }
        
        public void Reset()
        {
            _bucketIndex = 0;
            _current = null;
            _started = false;
        }

        private Node? FindNextNode()
        {
            while (_bucketIndex < _collection._buckets.Length)
            {
                if (_collection._buckets[_bucketIndex] != null)
                    return _collection._buckets[_bucketIndex];
                _bucketIndex++;
            }

            return null;
        }
    }
    
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