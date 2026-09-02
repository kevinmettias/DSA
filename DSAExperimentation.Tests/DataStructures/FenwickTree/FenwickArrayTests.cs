using DSAExperimentation.DataStructures.FenwickTree;

namespace DSAExperimentation.Tests.DataStructures.FenwickTree;

public sealed class FenwickArrayTests
{
    [Fact]
    public void Set_ThenGet_ReturnsTheStoredValue()
    {
        var array = new FenwickArray<int>(4);

        array.Set(3, 42);

        Assert.Equal(42, array.Get(3));
    }

    [Fact]
    public void Set_OverwritesAPreviousValueAtTheSameIndex()
    {
        var array = new FenwickArray<int>(4);

        array.Set(1, 7);
        array.Set(1, 9);

        Assert.Equal(9, array.Get(1));
    }

    [Fact]
    public void Set_LeavesOtherIndicesUntouched()
    {
        var array = new FenwickArray<int>(4);

        array.Set(2, 5);

        Assert.Equal(0, array.Get(1));
        Assert.Equal(0, array.Get(3));
    }

    [Fact]
    public void Set_AcceptsTheHighestOneBasedIndexTheSizeAllows()
    {
        // The backing array is size + 1 long precisely so index `size` is valid.
        var array = new FenwickArray<int>(4);

        array.Set(4, 11);

        Assert.Equal(11, array.Get(4));
    }

    [Fact]
    public void Set_PastTheEnd_Throws()
    {
        var array = new FenwickArray<int>(4);

        Assert.Throws<IndexOutOfRangeException>(() => array.Set(5, 1));
    }

    [Fact]
    public void Set_WorksForReferenceElements()
    {
        var array = new FenwickArray<string>(2);

        array.Set(1, "value");

        Assert.Equal("value", array.Get(1));
    }

    [Fact]
    public void Get_UnsetIndex_ReturnsTheDefaultElement()
    {
        Assert.Equal(0, new FenwickArray<int>(4).Get(2));
    }

    [Fact]
    public void Get_AfterSet_ReturnsThatIndexAndNotItsNeighbours()
    {
        var array = new FenwickArray<int>(4);

        array.Set(2, 5);

        Assert.Equal(5, array.Get(2));
        Assert.Equal(0, array.Get(1));
    }

    [Fact]
    public void Get_IndexZero_IsTheUnusedOneBasedPadSlot()
    {
        Assert.Equal(0, new FenwickArray<int>(4).Get(0));
    }
}
