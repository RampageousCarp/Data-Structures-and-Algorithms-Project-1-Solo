using Project1.Services.Interfaces;

namespace Project1.Services.Collections;

public class MyLinkedListCollection<T> : IMyCollection<T>
{
    private Node? _head;
    private Node? _tail;
    protected int _count;
    protected bool _dirty = false;
    protected int _dirtyCount { get; set; }

    public MyLinkedListCollection()
    {
        _count = 0;
        _dirty = false;
    }

    public MyLinkedListCollection(IMyIterator<T> iterator)
    {
        _dirty = false;
        while (iterator.HasNext())
            AddLast(iterator.Next());
    }

    #region Methodes

    public void Add(T item)
    {
        throw new NotImplementedException();
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

    public void Sort(Comparison<T> comparison)
    {
        throw new NotImplementedException();
    }

    public int Count { get; }
    public bool Dirty { get; }
    public void IncreaseDirty()
    {
        throw new NotImplementedException();
    }

    public void ResetDirty()
    {
        throw new NotImplementedException();
    }

    public int GetDirtyCount()
    {
        throw new NotImplementedException();
    }

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

    #endregion

    #region Helpers

    private void AddLast(T item)
    {
        throw new NotImplementedException();
    }

    #endregion
    
    #region InnerClasses

    private class Node(T data, Node? prev = null, Node? next = null)
    {
        public T Data = data;
        public Node? Next = next; 
        public Node? Prev = prev;
    }
    
    private class MyLinkedListCollectionIterator : IMyIterator<T>
    {
        private readonly MyLinkedListCollection<T> _collection;
        private Node? _current;
        private bool _started;
        
        public MyLinkedListCollectionIterator(MyLinkedListCollection<T> collection)
        {
            _collection = collection;
            _current = null;
            _started = false;
        }
        
        public bool HasNext()
        {
            if (!_started)
                return _collection._head != null;

            return _current?.Next != null;
        }
        
        public T Next()
        {
            if (!_started)
            {
                _started = true;
                _current = _collection._head;
            }
            else
                _current = _current?.Next;
            
            return _current != null ? _current.Data : default!;
        }
        
        public void Reset()
        {
            _current = null;
            _started = false;
        }
    }

    #endregion
}