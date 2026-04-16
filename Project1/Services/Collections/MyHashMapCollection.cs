using Project1.Services.Interfaces;

namespace Project1.Services.Collections;

public class MyHashMapCollection<T> : IMyCollection<T>
{
    
    public MyHashMapCollection()
    {
    }

    public MyHashMapCollection(IMyIterator<T> iterator)
    {
        
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

    #endregion
    
    
    #region InnerClasses
    private class Node(T data)
    {
        public T Data = data;
        public Node? Next = null;
    }
    #endregion
}