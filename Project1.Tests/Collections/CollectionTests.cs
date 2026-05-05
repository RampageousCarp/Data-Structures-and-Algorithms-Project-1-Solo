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

    #region Iterator

    [Fact]
    public void Iterator_EmptyCollection_HasNextIsFalse()
    {
        IMyCollection<T> col = CreateEmpty();
        IMyIterator<T> it = col.GetIterator();
        Assert.False(it.HasNext());
    }
 
    [Fact]
    public void Iterator_ThreeItems_IteratesAllItems()
    {
        IMyCollection<T> col = CreateEmpty();
        col.Add(Item1);
        col.Add(Item2);
        col.Add(Item3);
 
        IMyIterator<T> it = col.GetIterator();
        int count = 0;
        while (it.HasNext())
        {
            it.Next();
            count++;
        }
        Assert.Equal(3, count);
    }
 
    [Fact]
    public void Iterator_Reset_IteratesAgainFromStart()
    {
        IMyCollection<T> col = CreateEmpty();
        col.Add(Item1);
        col.Add(Item2);
 
        IMyIterator<T> it = col.GetIterator();
        while (it.HasNext())
            it.Next();
 
        it.Reset();
        Assert.True(it.HasNext());
 
        int countAfterReset = 0;
        while (it.HasNext())
        {
            it.Next();
            countAfterReset++;
        }
        Assert.Equal(2, countAfterReset);
    }

    #endregion

    #region Constructor from iterator

    [Fact]
    public void ConstructFromIterator_CopiesAllItems()
    {
        IMyCollection<T> source = CreateEmpty();
        source.Add(Item1);
        source.Add(Item2);
        source.Add(Item3);
 
        IMyCollection<T> copy = CreateFromIterator(source.GetIterator());
        Assert.Equal(3, copy.Count);
    }

    #endregion

    #region Dirty

    [Fact]
    public void Dirty_InitiallyFalse()
    {
        IMyCollection<T> col = CreateEmpty();
        Assert.False(col.Dirty);
    }
 
    [Fact]
    public void Dirty_AfterAdd_IsTrue()
    {
        IMyCollection<T> col = CreateEmpty();
        col.Add(Item1);
        Assert.True(col.Dirty);
    }
 
    [Fact]
    public void ResetDirty_ClearsDirtyFlag()
    {
        IMyCollection<T> col = CreateEmpty();
        col.Add(Item1);
        col.ResetDirty();
        Assert.False(col.Dirty);
        Assert.Equal(0, col.GetDirtyCount());
    }
 
    [Fact]
    public void IncreaseDirty_IncrementsCount()
    {
        IMyCollection<T> col = CreateEmpty();
        col.IncreaseDirty();
        col.IncreaseDirty();
        Assert.Equal(2, col.GetDirtyCount());
        Assert.True(col.Dirty);
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
    
    [Fact]
    public void Sort_Ascending_ItemsInOrder()
    {
        MyArrayCollection<int> col = new MyArrayCollection<int>();
        col.Add(30);
        col.Add(10);
        col.Add(20);
        col.Sort((a, b) => a.CompareTo(b));
 
        IMyIterator<int> it = col.GetIterator();
        List<int> items = new List<int>();
        while (it.HasNext())
            items.Add(it.Next());
 
        Assert.Equal(new [] { 10, 20, 30 }, items);
    }
 
    [Fact]
    public void Sort_NullComparison_DoesNotThrow()
    {
        MyArrayCollection<int> col = new MyArrayCollection<int>();
        col.Add(1);
        col.Add(2);
        
        var ex = Record.Exception(() => col.Sort(null));
        
        Assert.Null(ex);
    }
    
    [Fact]
    public void Add_BeyondDefaultCapacity_ExpandsCorrectly()
    {
        MyArrayCollection<int> col = new MyArrayCollection<int>();
        for (int i = 0; i < 70; i++)
            col.Add(i);
        
        Assert.Equal(70, col.Count);
    }

}

public class MyLinkedListCollectionTests : MyCollectionTests<int>
{
    protected override IMyCollection<int> CreateEmpty() => new MyLinkedListCollection<int>();
    protected override IMyCollection<int> CreateFromIterator(IMyIterator<int> it) => new MyLinkedListCollection<int>(it);
    protected override int Item1 => 10;
    protected override int Item2 => 20;
    protected override int Item3 => 30;
    
    [Fact]
    public void Sort_Ascending_ItemsInOrder()
    {
        MyLinkedListCollection<int> col = new MyLinkedListCollection<int>();
        col.Add(30);
        col.Add(10);
        col.Add(20);
        col.Sort((a, b) => a.CompareTo(b));
 
        IMyIterator<int> it = col.GetIterator();
        List<int> items = new List<int>();
        while (it.HasNext())
            items.Add(it.Next());
 
        Assert.Equal(new[] { 10, 20, 30 }, items);
    }
 
    [Fact]
    public void Sort_Descending_ItemsInOrder()
    {
        MyLinkedListCollection<int> col = new MyLinkedListCollection<int>();
        col.Add(10);
        col.Add(30);
        col.Add(20);
        col.Sort((a, b) => b.CompareTo(a));
 
        IMyIterator<int> it = col.GetIterator();
        List<int> items = new List<int>();
        while (it.HasNext())
            items.Add(it.Next());
 
        Assert.Equal(new[] { 30, 20, 10 }, items);
    }
 
    [Fact]
    public void Remove_LastItem_TailIsUpdated()
    {
        MyLinkedListCollection<int> col = new MyLinkedListCollection<int>();
        col.Add(10);
        col.Add(20);
        col.Remove(20);
        Assert.Equal(1, col.Count);
 
        IMyIterator<int> it = col.GetIterator();
        
        Assert.True(it.HasNext());
        Assert.Equal(10, it.Next());
    }
    
}

public class MyBSTCollectionTests : MyCollectionTests<int>
{
    private static readonly Comparison<int> Cmp = (a, b) => a.CompareTo(b);

    protected override IMyCollection<int> CreateEmpty() => new MyBSTCollection<int>(Cmp);
    protected override IMyCollection<int> CreateFromIterator(IMyIterator<int> it) => new MyBSTCollection<int>(it, Cmp);
    protected override int Item1 => 10;
    protected override int Item2 => 20;
    protected override int Item3 => 30;
    
    [Fact]
    public void Iterator_ReturnsItemsInSortedOrder()
    {
        MyBSTCollection<int> col = new MyBSTCollection<int>(Cmp);
        col.Add(30);
        col.Add(10);
        col.Add(20);
 
        IMyIterator<int> it = col.GetIterator();
        List<int> items = new List<int>();
        while (it.HasNext())
            items.Add(it.Next());
 
        Assert.Equal(new[] { 10, 20, 30 }, items);
    }
 
    [Fact]
    public void Add_DuplicateItem_IgnoresDuplicate()
    {
        MyBSTCollection<int> col = new MyBSTCollection<int>(Cmp);
        col.Add(10);
        col.Add(10);
        
        Assert.Equal(1, col.Count);
    }
 
    [Fact]
    public void Remove_RootNode_TreeRemainsValid()
    {
        MyBSTCollection<int> col = new MyBSTCollection<int>(Cmp);
        col.Add(20);
        col.Add(10);
        col.Add(30);
        col.Remove(20);
        Assert.Equal(2, col.Count);
 
        int result = col.FindBy(10, (item, key) => item.CompareTo(key));
        
        Assert.Equal(10, result);
    }
    
}

public class MyBSTCollectionSortableTests : MyCollectionTests<int>
{
    private static readonly Comparison<int> Cmp = (a, b) => a.CompareTo(b);

    protected override IMyCollection<int> CreateEmpty() => new MyBSTCollectionSortable<int>(Cmp);
    protected override IMyCollection<int> CreateFromIterator(IMyIterator<int> it) => new MyBSTCollectionSortable<int>(it, Cmp);
    protected override int Item1 => 10;
    protected override int Item2 => 20;
    protected override int Item3 => 30;
    
    [Fact]
    public void Sort_ThenIterator_UsesSnapshotOrder()
    {
        MyBSTCollectionSortable<int> col = new MyBSTCollectionSortable<int>(Cmp);
        col.Add(30);
        col.Add(10);
        col.Add(20);
        col.Sort((a, b) => b.CompareTo(a)); 
 
        IMyIterator<int> it = col.GetIterator();
        List<int> items = new List<int>();
        while (it.HasNext())
            items.Add(it.Next());
 
        Assert.Equal(new[] { 30, 20, 10 }, items);
    }
 
    [Fact]
    public void Sort_AfterAdd_InvalidatesSnapshot()
    {
        MyBSTCollectionSortable<int> col = new MyBSTCollectionSortable<int>(Cmp);
        col.Add(30);
        col.Add(10);
        col.Sort((a, b) => a.CompareTo(b));
 
        col.Add(20);
        col.Sort((a, b) => a.CompareTo(b));
 
        IMyIterator<int> it = col.GetIterator();
        List<int> items = new List<int>();
        while (it.HasNext())
            items.Add(it.Next());
 
        Assert.Equal(new[] { 10, 20, 30 }, items);
    }
 
    [Fact]
    public void Sort_CalledTwice_NoError()
    {
        MyBSTCollectionSortable<int> col = new MyBSTCollectionSortable<int>(Cmp);
        col.Add(30);
        col.Add(10);
        col.Sort((a, b) => a.CompareTo(b));
        
        var ex = Record.Exception(() => col.Sort((a, b) => a.CompareTo(b)));
        
        Assert.Null(ex);
    }
    
}

public class MyHashMapCollectionTests : MyCollectionTests<int>
{
    protected override IMyCollection<int> CreateEmpty() => new MyHashMapCollection<int>();
    protected override IMyCollection<int> CreateFromIterator(IMyIterator<int> it) => new MyHashMapCollection<int>(it);
    protected override int Item1 => 10;
    protected override int Item2 => 20;
    protected override int Item3 => 30;
    
    [Fact]
    public void Add_DuplicateItem_IgnoresDuplicate()
    {
        MyHashMapCollection<int> col = new MyHashMapCollection<int>();
        col.Add(10);
        col.Add(10);
        
        Assert.Equal(1, col.Count);
    }
 
    [Fact]
    public void Add_BeyondLoadFactor_ResizesCorrectly()
    {
        MyHashMapCollection<int> col = new MyHashMapCollection<int>();
        for (int i = 0; i < 20; i++)
            col.Add(i);
        
        Assert.Equal(20, col.Count);
    }
    
}

public class MyHashMapCollectionSortableTests : MyCollectionTests<int>
{
    protected override IMyCollection<int> CreateEmpty() => new MyHashMapCollectionSortable<int>();
    protected override IMyCollection<int> CreateFromIterator(IMyIterator<int> it) => new MyHashMapCollectionSortable<int>(it);
    protected override int Item1 => 10;
    protected override int Item2 => 20;
    protected override int Item3 => 30;
    
    [Fact]
    public void Sort_ThenIterator_UsesSnapshotOrder()
    {
        MyHashMapCollectionSortable<int> col = new MyHashMapCollectionSortable<int>();
        col.Add(30);
        col.Add(10);
        col.Add(20);
        col.Sort((a, b) => a.CompareTo(b));
 
        IMyIterator<int> it = col.GetIterator();
        List<int> items = new List<int>();
        while (it.HasNext())
            items.Add(it.Next());
 
        Assert.Equal(new[] { 10, 20, 30 }, items);
    }
 
    [Fact]
    public void Sort_AfterRemove_InvalidatesSnapshot()
    {
        MyHashMapCollectionSortable<int> col = new MyHashMapCollectionSortable<int>();
        col.Add(30);
        col.Add(10);
        col.Add(20);
        col.Sort((a, b) => a.CompareTo(b));
 
        col.Remove(30);
        col.Sort((a, b) => a.CompareTo(b));
 
        IMyIterator<int> it = col.GetIterator();
        List<int> items = new List<int>();
        while (it.HasNext())
            items.Add(it.Next());
 
        Assert.Equal(2, items.Count);
        Assert.DoesNotContain(30, items);
    }
 
    [Fact]
    public void Sort_NullComparison_InvalidatesSnapshot()
    {
        MyHashMapCollectionSortable<int> col = new MyHashMapCollectionSortable<int>();
        col.Add(10);
        col.Add(20);
        col.Sort(null);
        
        Assert.Equal(2, col.Count);
    }
    
}
