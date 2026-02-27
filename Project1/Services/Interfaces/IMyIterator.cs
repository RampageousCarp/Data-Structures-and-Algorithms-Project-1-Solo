namespace Project1.Services.Interfaces;

public interface IMyIterator<T> { 
    bool HasNext(); // Checks if there is another element
    T Next(); // Returns the next element
    void Reset(); // Resets the iterator to the beginning
}