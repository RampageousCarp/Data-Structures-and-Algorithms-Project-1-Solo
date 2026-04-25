using Project1.Services.Factories;
using Project1.Services.Interfaces;

namespace Project1.Services.Collections;

public class MyBSTCollection<T>: IMyCollection<T>
{
    
    protected Node? _root;
    protected int _count;
    private bool _dirty;
    private int _dirtyCount;
    protected readonly Comparison<T> _defaultComparison;
    
    public MyBSTCollection(Comparison<T> defaultComparison)
    {
        _defaultComparison = defaultComparison;
    }

    public MyBSTCollection(IMyIterator<T> iterator, Comparison<T> defaultComparison)
    {
        _defaultComparison = defaultComparison;
        while (iterator.HasNext())
            Add(iterator.Next());
        
        _dirty = false;
    }

    
    #region Methodes
    public virtual void Add(T item)
    {
        _root = InsertNode(_root, item);
        IncreaseDirty();
    }

    public virtual void Remove(T item)
    {
        (_root, bool removed) = RemoveNode(_root, item);
        if (removed)
        {
            _count--;
            IncreaseDirty();
        }
    }

    public T? FindBy<K>(K key, Func<T, K, int> comparer)
    {
        return FindInOrder(_root, key, comparer);
    }

    public virtual IMyCollection<T> Filter(Func<T, bool> predicate)
    {
        MyBSTCollection<T> filtered = new MyBSTCollection<T>(_defaultComparison);
        FilterInOrder(_root, predicate, filtered);
        
        return filtered;
    }

    public virtual void Sort(Comparison<T>? comparison)
    {
        return ;
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
        return ReduceInOrder(_root, acc, accumulator);
    }

    public R Reduce<R>(R initial, Func<R, T, R> accumulator)
    {
        return ReduceInOrder(_root, initial, accumulator);
    }

    public virtual IMyIterator<T> GetIterator()
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

    private (Node? node, bool removed) RemoveNode(Node? node, T item)
    {
        if (node == null)
            return (null, true);

        int cmp = _defaultComparison(item, node.Data);

        if (cmp < 0)
        {
            (Node? newLeft, bool removed) = RemoveNode(node.Left, item);
            node.Left = newLeft;
            return (node, removed);
        }
        
        else if (cmp > 0)
        {
            (Node? newRight, bool removed) = RemoveNode(node.Right, item);
            node.Right = newRight;
            return (node, removed);
        }

        else
        {
            if (node.Left == null)
                return (node.Right, true);
            if (node.Right == null)
                return (node.Left, true);

            Node successor = GetSuccessor(node.Right);
            node.Data = successor.Data;

            (Node? newRight, _) = RemoveNode(node.Right, successor.Data);
            node.Right = newRight;

            return (node, true);
        }
    }

    private Node GetSuccessor(Node node)
    {
        Node curr = node;
        while (curr.Left != null)
            curr = curr.Left;

        return curr;
    }

    private T? FindInOrder<K>(Node? node, K key, Func<T, K, int> comparer)
    {
        if (node == null)
            return default!;

        T? left = FindInOrder(node.Left, key, comparer);
        if (left is not null)
            return left;

        if (comparer(node.Data, key) == 0)
            return node.Data;
        
        return FindInOrder(node.Right, key, comparer);
    }

    protected void FilterInOrder(Node? node, Func<T, bool> predicate, MyBSTCollection<T> filtered)
    {
        if (node == null)
            return;
        
        FilterInOrder(node.Left, predicate, filtered);
        if (predicate(node.Data))
            filtered.Add(node.Data);
        FilterInOrder(node.Right, predicate, filtered);
    }

    private R ReduceInOrder<R>(Node? node, R acc, Func<R, T, R> accumulator)
    {
        if (node == null)
            return acc;

        acc = ReduceInOrder(node.Left, acc, accumulator);
        acc = accumulator(acc, node.Data);
        acc = ReduceInOrder(node.Right, acc, accumulator);

        return acc;
    }

    #endregion

    #region InnerClasses
    
    protected class Node(T data)
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


    #endregion
}