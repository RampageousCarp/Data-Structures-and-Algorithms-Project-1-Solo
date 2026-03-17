using Project1.Services.Interfaces;

namespace Project1.Repositories.Interfaces;
interface IGenericRepository<T>
{
    T[] LoadItems();
    void SaveItems(IMyIterator<T> items, int count);
}