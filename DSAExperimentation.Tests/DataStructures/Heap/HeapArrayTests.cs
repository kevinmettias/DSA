using DSAExperimentation.DataStructures.Heap;

namespace DSAExperimentation.Tests.DataStructures.Heap;

public sealed partial class HeapArrayTests
{
    [Fact]
    public void Add_IncreasesCountAndStoresValueAtEnd()
    {
        var array = new HeapArray<int>();

        array.Add(1);
        array.Add(2);

        Assert.Equal(2, array.Count);
        Assert.Equal(1, array.Get(0));
        Assert.Equal(2, array.Get(1));
    }

    [Fact]
    public void Set_OverwritesValueAtIndex()
    {
        var array = new HeapArray<int>();
        array.Add(1);

        array.Set(0, 42);

        Assert.Equal(42, array.Get(0));
    }

    [Fact]
    public void Swap_ExchangesValuesAtBothIndexes()
    {
        var array = new HeapArray<int>();
        array.Add(1);
        array.Add(2);

        array.Swap(0, 1);

        Assert.Equal(2, array.Get(0));
        Assert.Equal(1, array.Get(1));
    }

    [Fact]
    public void RemoveLast_DecreasesCountAndDropsFinalElement()
    {
        var array = new HeapArray<int>();
        array.Add(1);
        array.Add(2);

        array.RemoveLast();

        Assert.Equal(1, array.Count);
        Assert.Equal(1, array.Get(0));
    }

    [Fact]
    public void Count_ReflectsAddsFollowedByRemove()
    {
        var array = new HeapArray<int>();

        Assert.Equal(0, array.Count);

        array.Add(1);
        array.Add(2);
        array.RemoveLast();

        Assert.Equal(1, array.Count);
    }

    [Fact]
    public void Get_ReturnsTheElementStoredAtThatSlot()
    {
        var array = new HeapArray<int>();

        array.Add(7);
        array.Add(9);

        Assert.Equal(7, array.Get(0));
        Assert.Equal(9, array.Get(1));
    }

    [Fact]
    public void Get_AfterASwap_ReflectsTheNewPositions()
    {
        var array = new HeapArray<int>();
        array.Add(7);
        array.Add(9);

        array.Swap(0, 1);

        Assert.Equal(9, array.Get(0));
        Assert.Equal(7, array.Get(1));
    }
}
