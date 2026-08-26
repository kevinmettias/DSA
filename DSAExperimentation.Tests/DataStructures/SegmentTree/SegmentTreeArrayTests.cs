using DSAExperimentation.DataStructures.SegmentTree;

namespace DSAExperimentation.Tests.DataStructures.SegmentTree;

public sealed partial class SegmentTreeArrayTests
{
    [Fact]
    public void Set_ThenGet_ReturnsStoredValue()
    {
        var array = new SegmentTreeArray<int>(4);

        array.Set(3, 42);

        Assert.Equal(42, array.Get(3));
    }

    [Fact]
    public void Get_DefaultsToZeroValueBeforeAnySet()
    {
        var array = new SegmentTreeArray<int>(4);

        Assert.Equal(0, array.Get(0));
    }
}
