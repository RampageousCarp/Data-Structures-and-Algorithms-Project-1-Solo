namespace Project1.Services.Interfaces;

public interface IMyCollection<T> {
    void Add(T item);
    void Remove(T item);
    T? FindBy<K>(K key, Func<T, K, int> comparer);
    IMyCollection<T> Filter(Func<T, bool> predicate);
    void Sort(Comparison<T> comparison);
    int Count { get; }
    bool Dirty { get; }
    void IncreaseDirty();
    void ResetDirty();
    int GetDirtyCount();
    R Reduce<R>(Func<R, T, R> accumulator);
    // OR
    R Reduce<R>(R initial, Func<R, T, R> accumulator);
    IMyIterator<T> GetIterator(); // Custom Iterator - Since we are not using System.Collections.Generic
}