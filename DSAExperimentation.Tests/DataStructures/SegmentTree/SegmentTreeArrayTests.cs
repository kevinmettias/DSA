using DSAExperimentation.DataStructures.SegmentTree;

namespace DSAExperimentation.Tests.DataStructures.SegmentTree;

public sealed partial class SegmentTreeArrayTests
{
    private const int ArraySize = 4;
    private const int StoredIndex = 3;
    private const int StoredValue = 42;
    private const int FillValue = 123;

    [Fact]
    public void Set_ThenGet_ReturnsStoredValue()
    {
        var array = new SegmentTreeArray<int>(ArraySize);

        array.Set(StoredIndex, StoredValue);

        Assert.Equal(StoredValue, array.Get(StoredIndex));
    }

    [Fact]
    public void Get_DefaultsToZeroValueBeforeAnySet()
    {
        var array = new SegmentTreeArray<int>(ArraySize);

        Assert.Equal(0, array.Get(0));
    }

    // ArraySize is beyond the leaves of a four-leaf tree, so this slot is one no build recursion
    // would ever reach - the whole point of seeding the arena uniformly rather than at the nodes a
    // tree happens to visit (LazySegmentTree's pending tags rely on it).
    [Fact]
    public void Constructor_WithFill_SeedsSlotsNoRecursionWouldVisit()
    {
        var array = new SegmentTreeArray<int>(ArraySize, FillValue);

        Assert.Equal(FillValue, array.Get(0));
        Assert.Equal(FillValue, array.Get(ArraySize));
    }

    [Fact]
    public void Set_AfterFill_OverwritesOnlyThatSlot()
    {
        var array = new SegmentTreeArray<int>(ArraySize, FillValue);

        array.Set(StoredIndex, StoredValue);

        Assert.Equal(StoredValue, array.Get(StoredIndex));
        Assert.Equal(FillValue, array.Get(ArraySize));
    }
}
