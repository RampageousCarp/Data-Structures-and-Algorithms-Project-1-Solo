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
        AddLast(item);
        SetDirty();
    }

    public void Remove(T item)
    {
        Node? current = _head;
        while (current != null)
        {
            if (current.Data!.Equals(item))
            {
                Unlink(current);
                return;
            }

            current = current.Next;
        }
    }

    public T? FindBy<K>(K key, Func<T, K, int> comparer)
    {
        Node? current = _head;

        while (current is not null)
        {
            if (comparer(current.Data, key) == 0)
                return current.Data;

            current = current.Next;
        }

        return default;
    }

    public IMyCollection<T> Filter(Func<T, bool> predicate)
    {
        MyLinkedListCollection<T> filteredCollection = new MyLinkedListCollection<T>();

        Node? current = _head;

        while (current is not null)
        {
            if (predicate(current.Data))
                filteredCollection.Add(current.Data);
            current = current.Next;
        }

        return filteredCollection;
    }

    public void Sort(Comparison<T>? comparison)
    {
        if (comparison is not null)
        {
            if (_count > 1)
                _head = MergeSort(_head, comparison);

            RepairPrevOrder();
        }
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
        _dirtyCount = 0;
        _dirty = false;
    }

    public int GetDirtyCount() => _dirtyCount;

    public R Reduce<R>(Func<R, T, R> accumulator)
    {
        R acc = default(R);
        Node? current = _head;

        while (current is not null)
        {
            acc = accumulator(acc, current.Data);
            current = current.Next;
        }

        return acc;
    }

    public R Reduce<R>(R initial, Func<R, T, R> accumulator)
    {
        R acc = initial;
        Node? current = _head;

        while (current is not null)
        {
            acc = accumulator(acc, current.Data);
            current = current.Next;
        }

        return acc;
    }

    public IMyIterator<T> GetIterator() => new MyLinkedListCollectionIterator(this);

    #endregion

    #region Helpers
    
    private void SetDirty() => _dirty = true;

    private void AddLast(T item)
    {
        Node newNode = new Node(item, _tail, null);
        if (_tail == null)
            _head = _tail = newNode;
        else
        {
            _tail.Next = newNode;
            _tail = newNode;
        }

        _count++;
    }

    private void Unlink(Node node)
    {
        if (node.Prev != null)
            node.Prev.Next = node.Next;
        else
            _head = node.Next;
        
        if (node.Next != null)
            node.Next.Prev = node.Prev;
        else
            _tail = node.Prev;

        _count--;
    }

    private Node? MergeSort(Node? head, Comparison<T> comparison)
    {
        if (head?.Next == null)
            return head;

        Node? mid = GetMiddle(head);
        Node? secondHalf = mid!.Next;
        mid.Next = null;

        Node? left  = MergeSort(head, comparison);
        Node? right = MergeSort(secondHalf, comparison);

        return Merge(left, right, comparison);
    }

    private static Node? GetMiddle(Node? head)
    {
        Node? slow = head;
        Node? fast = head?.Next;

        while (fast?.Next != null)
        {
            slow = slow!.Next;
            fast = fast.Next.Next;
        }

        return slow;
    }

    private Node? Merge(Node left, Node right, Comparison<T> comparison)
    {
        Node dummy = new(default!);
        Node? tail = dummy;

        while (left != null && right != null)
        {
            if (comparison(left.Data, right.Data) <= 0)
            {
                tail.Next = left;
                left = left.Next;
            }
            else
            {
                tail.Next = right;
                right = right.Next;
            }
            tail = tail.Next;
        }

        tail.Next = left ?? right;
        return dummy.Next;
    }

    private void RepairPrevOrder()
    {
        Node? current = _head;
        Node? prev = null;

        while (current != null)
        {
            current.Prev = prev;
            prev = current;
            current = current.Next;
        }

        _tail = prev;
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