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
            bool inserted = InsertNode(item);
    
            if (inserted)
            {
                IncreaseDirty();
                InvalidateSnapshot();
            }
        }

        public void Remove(T item)
        {
            int index = GetBucketIndex(item);
            Node? current = _buckets[index];
            Node? prev = null;

            while (current != null)
            {
                if (current!.Data.Equals(item))
                {
                    if (prev == null)
                        _buckets[index] = current.Next;
                    else
                        prev.Next = current.Next;
                    _count--;
                    InvalidateSnapshot();
                    return;
                }

                prev = current;
                current = current.Next;
            }
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
            if (_isSorted) return;
            
            T[] items = ToArray();
            QuickSort(items, 0, _count - 1, comparison);

            _sortedSnapshot = items;        // snapshot stored
            _sortComparison = comparison;
            _isSorted = true;
        }

        public int Count => _count;
        public bool Dirty => _dirty;
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

    private T[] ToArray()
    {
        T[] arrayItems = new T[_count];

        MyHashMapCollectionIterator iterator = new MyHashMapCollectionIterator(this);

        iterator.Reset();
        int p = 0;
        while (iterator.HasNext())
            arrayItems[p++] = iterator.Next();

        return arrayItems;
    }

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
        _count = 0;

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

    private bool InsertNode(T? item)
    {
        int index = GetBucketIndex(item!);
        Node? current = _buckets[index];

        if (current == null)
        {
            _buckets[index] = new Node(item!);
            _count++;
            return true;
        }

        while (current.Next != null)
        {
            if (current.Data!.Equals(item))
                return false;

            current = current.Next;
        }

        if (!current.Data!.Equals(item))
        {
            current.Next = new Node(item!);
            _count++;
            return true;
        }

        return false;
    }

    private void InvalidateSnapshot()
    {
        _sortedSnapshot = null;
        _isSorted = false;
        _sortComparison = null;
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
            
            int savedIndex = _bucketIndex;
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