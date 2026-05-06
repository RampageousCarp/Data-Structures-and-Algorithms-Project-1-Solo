using Project1.Services;

namespace Project1.Tests.Collections;

public class ArrayIteratorTests
{
    [Fact]
    public void HasNext_OnNonEmptyArray_IsTrue()
    {
        int[] items = [1, 2, 3];
        ArrayIterator<int> it = new ArrayIterator<int>(items, 3);
        
        Assert.True(it.HasNext());
    }
 
    [Fact]
    public void Next_IteratesInOrder()
    {
        int[] items = [10, 20, 30];
        ArrayIterator<int> it = new ArrayIterator<int>(items, 3);
        
        Assert.Equal(10, it.Next());
        Assert.Equal(20, it.Next());
        Assert.Equal(30, it.Next());
    }
 
    [Fact]
    public void HasNext_AfterExhausted_IsFalse()
    {
        var items = new[] { 1 };
        var it = new ArrayIterator<int>(items, 1);
        it.Next();
        Assert.False(it.HasNext());
    }
 
    [Fact]
    public void Reset_AllowsReIteration()
    {
        int[] items = [1, 2];
        ArrayIterator<int> it = new ArrayIterator<int>(items, 2);
        while (it.HasNext())
            it.Next();
        it.Reset();
        
        Assert.True(it.HasNext());
        Assert.Equal(1, it.Next());
    }
 
    [Fact]
    public void GetCount_ReturnsCorrectCount()
    {
        int[] items = [1, 2, 3, 4, 5];
        ArrayIterator<int> it = new ArrayIterator<int>(items, 5);
        
        Assert.Equal(5, it.GetCount());
    }
}