using Project1.Services.Interfaces;

namespace Project1.Services.Collections;

public class MyBSTCollection<T>: IMyCollection<T>
{
    
    private Node? _root;
    private int _count;
    private bool _dirty;
    private int _dirtyCount;
    private readonly Comparison<T> _defaultComparison;
    
    public MyBSTCollection(Comparison<T> defaultComparison)
    {
        _defaultComparison = defaultComparison;
    }

    public MyBSTCollection(IMyIterator<T> iterator, Comparison<T> defaultComparison)
    {
        _defaultComparison = defaultComparison;
        while (iterator.HasNext())
        {
            Add(iterator.Next());
        }
        
        _dirty = false;
    }

    
    #region Methodes
    public void Add(T item)
    {
        _root = InsertNode(_root, item);
        IncreaseDirty();
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
        return new BSTIterator(this);
    }
    
    #endregion

    #region Helpers

    private Node InsertNode(Node? node, T item)
    {
        if (node == null)
        {
            _count++;
            return new Node(item);
        }

        int cmp = _defaultComparison(item, node.Data);

        if (cmp < 0)
            node.Left = InsertNode(node.Left, item);
        else if (cmp > 0)
            node.Right = InsertNode(node.Right, item);

        return node;
    }

    #endregion

    #region InnerClasses
    
    private class Node(T data)
    {
        public T Data = data;
        public Node? Left = null!;
        public Node? Right = null!;
    }

    private class BSTIterator: IMyIterator<T>
    {
        private readonly MyBSTCollection<T> _collection;
        private readonly NodeStack _stack = new NodeStack();
        
        public BSTIterator(MyBSTCollection<T> collection)
        {
            _collection = collection;
            PushLeft(_collection._root);
        }

        public bool HasNext() => !_stack.IsEmpty;

        public T Next()
        {
            if (!HasNext())
                return default!;

            Node node = _stack.Pop();
            PushLeft(node.Right);
            return node.Data;
        }

        public void Reset()
        {
            _stack.Clear();
            PushLeft(_collection._root);
        }

        private void PushLeft(Node? node)
        {
            while (node != null)
            {
                _stack.Push(node);
                node = node.Left;
            }
        }
        
        private class NodeStack
        {
            private Node?[] _items = new Node[16];
            private int _top = -1;
 
            public bool IsEmpty => _top < 0;
 
            public void Push(Node node)
            {
                if (_top + 1 == _items.Length)
                    Array.Resize(ref _items, _items.Length * 2);
                
                _items[++_top] = node;
            }
 
            public Node Pop()
            {
                Node node = _items[_top]!;
                _items[_top--] = null;
                
                return node;
            }
 
            public void Clear()
            {
                for (int i = 0; i <= _top; i++)
                    _items[i] = null;
                
                _top = -1;
            }
        }
    }


    #endregion
}