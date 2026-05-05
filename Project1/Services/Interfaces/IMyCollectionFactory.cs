namespace Project1.Services.Interfaces;

public interface IMyCollectionFactory
{
    IMyCollection<T> Create<T>();
    IMyCollection<T> Create<T>(IMyIterator<T> iterator);
}