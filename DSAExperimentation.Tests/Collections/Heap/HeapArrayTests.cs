using DSAExperimentation.Collections.Heap;

namespace DSAExperimentation.Tests.Collections.Heap;

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
}
