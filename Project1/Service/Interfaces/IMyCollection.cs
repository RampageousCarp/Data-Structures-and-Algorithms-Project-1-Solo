interface IMyCollection<T> {
    void Add(T item);
    void Remove(T item);
    T FindBy<K>(K key, Func<T, K, bool> comparer);
    IMyCollection<T> Filter(Func<T, bool> predicate);
    void Sort(Comparison<T> comparison);
    int Count { get; }
    bool Dirty {get; set;}
    R Reduce<R>(Func<R, T, R> accumulator);
    // OR
    R Reduce<R>(R initial, Func<R, T, R> accumulator);
    IMyIterator<T> GetIterator(); // Custom Iterator - Since we are not using System.Collections.Generic
    IEnumerator<T> GetEnumerator(); // Extra foreach lookup.
}
interface IMyIterator<T> { 
    bool HasNext(); // Checks if there is another element
    T Next(); // Returns the next element
    void Reset(); // Resets the iterator to the beginning
}