using Project1.Services.Collections;
using Project1.Services.Interfaces;

namespace Project1.Tests.Collections;

public abstract class MyCollectionTests<T>
{
    protected abstract IMyCollection<T> CreateEmpty();
    protected abstract IMyCollection<T> CreateFromIterator(IMyIterator<T> iterator);
    protected abstract T Item1 { get; }
    protected abstract T Item2 { get; }
    protected abstract T Item3 { get; }


    #region Add and Count

    [Fact]
    public void Add_SingleItem_CountIsOne()
    {
        IMyCollection<T> col = CreateEmpty();
        col.Add(Item1);
        Assert.Equal(1, col.Count);
    }
 
    [Fact]
    public void Add_MultipleItems_CountIsCorrect()
    {
        IMyCollection<T> col = CreateEmpty();
        col.Add(Item1);
        col.Add(Item2);
        col.Add(Item3);
        Assert.Equal(3, col.Count);
    }

    #endregion

    #region Remove

    [Fact]
    public void Remove_ExistingItem_CountDecreases()
    {
        IMyCollection<T> col = CreateEmpty();
        col.Add(Item1);
        col.Add(Item2);
        col.Remove(Item1);
        Assert.Equal(1, col.Count);
    }
    
    [Fact]
    public void Remove_NonExistingItem_CountUnchanged()
    {
        IMyCollection<T> col = CreateEmpty();
        col.Add(Item1);
        col.Remove(Item2);
        Assert.Equal(1, col.Count);
    }
    
    [Fact]
    public void Remove_OnlyItem_CountIsZero()
    {
        IMyCollection<T> col = CreateEmpty();
        col.Add(Item1);
        col.Remove(Item1);
        Assert.Equal(0, col.Count);
    }

    #endregion

    #region FindBy

    [Fact]
    public void FindBy_ExistingItem_ReturnsItem()
    {
        IMyCollection<T> col = CreateEmpty();
        col.Add(Item1);
        col.Add(Item2);
 
        T? result = col.FindBy(Item1, (item, key) => item!.Equals(key) ? 0 : 1);
        Assert.Equal(Item1, result);
    }
    
    [Fact]
    public void FindBy_NonExistingItem_ReturnsDefault()
    {
        IMyCollection<T> col = CreateEmpty();
        col.Add(Item1);
 
        T? result = col.FindBy(Item3, (item, key) => item!.Equals(key) ? 0 : 1);
        Assert.Equal(default, result);
    }

    #endregion

    #region Filter

    [Fact]
    public void Filter_MatchingAll_ReturnsAllItems()
    {
        IMyCollection<T> col = CreateEmpty();
        col.Add(Item1);
        col.Add(Item2);
        col.Add(Item3);
 
        IMyCollection<T> filtered = col.Filter(_ => true);
        Assert.Equal(3, filtered.Count);
    }
 
    [Fact]
    public void Filter_MatchingNone_ReturnsEmpty()
    {
        IMyCollection<T> col = CreateEmpty();
        col.Add(Item1);
        col.Add(Item2);
 
        IMyCollection<T> filtered = col.Filter(_ => false);
        Assert.Equal(0, filtered.Count);
    }
 
    [Fact]
    public void Filter_MatchingSubset_ReturnsCorrectCount()
    {
        IMyCollection<T> col = CreateEmpty();
        col.Add(Item1);
        col.Add(Item2);
        col.Add(Item3);
 
        IMyCollection<T> filtered = col.Filter(item => item!.Equals(Item1));
        Assert.Equal(1, filtered.Count);
    }

    #endregion

    #region Reduce

    [Fact]
    public void Reduce_WithInitial_AggregatesCorrectly()
    {
        IMyCollection<T> col = CreateEmpty();
        col.Add(Item1);
        col.Add(Item2);
        col.Add(Item3);
 
        int count = col.Reduce(0, (acc, _) => acc + 1);
        Assert.Equal(3, count);
    }
 
    [Fact]
    public void Reduce_NoInitial_AggregatesCorrectly()
    {
        IMyCollection<T> col = CreateEmpty();
        col.Add(Item1);
        col.Add(Item2);
        col.Add(Item3);
 
        int count = col.Reduce<int>((acc, _) => acc + 1);
        Assert.Equal(3, count);
    }
 
    [Fact]
    public void Reduce_EmptyCollection_ReturnsInitial()
    {
        IMyCollection<T> col = CreateEmpty();
        int result = col.Reduce(42, (acc, _) => acc + 1);
        Assert.Equal(42, result);
    }

    #endregion
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
