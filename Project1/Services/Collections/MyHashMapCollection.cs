using Project1.Services.Interfaces;

namespace Project1.Services.Collections;

public class MyHashMapCollection<T> : IMyCollection<T>
{
    
    protected Node?[] _buckets;
    protected int _count;
    private bool _dirty;
    private int _dirtyCount;
    
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
    
    public virtual void Add(T item)
    {
        ResizeIfNeeded();
        bool inserted = InsertNode(item);

        if (inserted)
            IncreaseDirty();
    }

    public virtual void Remove(T item)
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
                return;
            }

            prev = current;
            current = current.Next;
        }
    }

    public T? FindBy<K>(K key, Func<T, K, int> comparer)
    {
        for (int i = 0; i < _buckets.Length; i++)
        {
            Node? current = _buckets[i];
            while (current != null)
            {
                if (comparer(current.Data, key) == 0)
                    return current.Data;

                current = current.Next;
            }
        }

        return default!;
    }

    public virtual IMyCollection<T> Filter(Func<T, bool> predicate)
    {
        IMyCollection<T> filtered = new MyHashMapCollection<T>();

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

    public virtual void Sort(Comparison<T>? comparison)
    {
        return;
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
        R acc = default!;

        for (int i = 0; i < _buckets.Length; i++)
        {
            Node? current = _buckets[i];
            while (current != null)
            {
                acc = accumulator(acc, current.Data);
                current = current.Next;
            }
        }

        return acc;
    }

    public R Reduce<R>(R initial, Func<R, T, R> accumulator)
    {
        R acc = initial;

        for (int i = 0; i < _buckets.Length; i++)
        {
            Node? current = _buckets[i];
            while (current != null)
            {
                acc = accumulator(acc, current.Data);
                current = current.Next;
            }
        }

        return acc;
    }

    public virtual IMyIterator<T> GetIterator()
    {
        return new MyHashMapCollectionIterator(this);
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
        
        Node? tail = null;
        while (current != null)
        {
            if (current.Data!.Equals(item))
                return false;

            tail = current;
            current = current.Next;
        }

        tail!.Next = new Node(item!);
        _count++;
        return true;
    }

    #endregion
    
    
    #region InnerClasses
    
    protected class Node(T data)
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
    #endregion
}