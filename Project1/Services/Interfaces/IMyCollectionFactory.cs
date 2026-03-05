namespace Project1.Services.Interfaces;

public interface IMyCollectionFactory
{
    IMyCollection<T> Create<T>();
}