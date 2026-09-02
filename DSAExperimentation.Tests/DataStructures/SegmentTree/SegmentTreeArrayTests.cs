using DSAExperimentation.DataStructures.SegmentTree;

namespace DSAExperimentation.Tests.DataStructures.SegmentTree;

public sealed partial class SegmentTreeArrayTests
{
    private const int ArraySize = 4;
    private const int StoredIndex = 3;
    private const int StoredValue = 42;

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
}
