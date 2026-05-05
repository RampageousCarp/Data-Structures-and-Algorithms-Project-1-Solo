using Project1.Services;
using Project1.Services.Collections;
using Project1.Services.Interfaces;
using Xunit;

namespace Project1.Tests;


public abstract class MyCollectionTests<T>
{
    protected abstract IMyCollection<T> CreateEmpty();
    protected abstract IMyCollection<T> CreateFromIterator(IMyIterator<T> iterator);
    protected abstract T Item1 { get; }
    protected abstract T Item2 { get; }
    protected abstract T Item3 { get; }
    

    
}

public class MyArrayCollectionTests : MyCollectionTests<int>
{
    protected override IMyCollection<int> CreateEmpty() => new MyArrayCollection<int>();
    protected override IMyCollection<int> CreateFromIterator(IMyIterator<int> it) => new MyArrayCollection<int>(it);
    protected override int Item1 => 10;
    protected override int Item2 => 20;
    protected override int Item3 => 30;

    
}

public class MyLinkedListCollectionTests : MyCollectionTests<int>
{
    protected override IMyCollection<int> CreateEmpty() => new MyLinkedListCollection<int>();
    protected override IMyCollection<int> CreateFromIterator(IMyIterator<int> it) => new MyLinkedListCollection<int>(it);
    protected override int Item1 => 10;
    protected override int Item2 => 20;
    protected override int Item3 => 30;
    
}

public class MyBSTCollectionTests : MyCollectionTests<int>
{
    private static readonly Comparison<int> Cmp = (a, b) => a.CompareTo(b);

    protected override IMyCollection<int> CreateEmpty() => new MyBSTCollection<int>(Cmp);
    protected override IMyCollection<int> CreateFromIterator(IMyIterator<int> it) => new MyBSTCollection<int>(it, Cmp);
    protected override int Item1 => 10;
    protected override int Item2 => 20;
    protected override int Item3 => 30;
    
}

public class MyBSTCollectionSortableTests : MyCollectionTests<int>
{
    private static readonly Comparison<int> Cmp = (a, b) => a.CompareTo(b);

    protected override IMyCollection<int> CreateEmpty() => new MyBSTCollectionSortable<int>(Cmp);
    protected override IMyCollection<int> CreateFromIterator(IMyIterator<int> it) => new MyBSTCollectionSortable<int>(it, Cmp);
    protected override int Item1 => 10;
    protected override int Item2 => 20;
    protected override int Item3 => 30;
    
}

public class MyHashMapCollectionTests : MyCollectionTests<int>
{
    protected override IMyCollection<int> CreateEmpty() => new MyHashMapCollection<int>();
    protected override IMyCollection<int> CreateFromIterator(IMyIterator<int> it) => new MyHashMapCollection<int>(it);
    protected override int Item1 => 10;
    protected override int Item2 => 20;
    protected override int Item3 => 30;
    
}

public class MyHashMapCollectionSortableTests : MyCollectionTests<int>
{
    protected override IMyCollection<int> CreateEmpty() => new MyHashMapCollectionSortable<int>();
    protected override IMyCollection<int> CreateFromIterator(IMyIterator<int> it) => new MyHashMapCollectionSortable<int>(it);
    protected override int Item1 => 10;
    protected override int Item2 => 20;
    protected override int Item3 => 30;
    
}
