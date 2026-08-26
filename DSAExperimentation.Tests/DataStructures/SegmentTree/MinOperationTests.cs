using DSAExperimentation.DataStructures.SegmentTree;

namespace DSAExperimentation.Tests.DataStructures.SegmentTree;

public sealed partial class MinOperationTests
{
    [Fact]
    public void Identity_IsMaxValue() => Assert.Equal(int.MaxValue, MinOperation<int>.Identity);

    [Fact]
    public void Combine_ReturnsSmallerValue() => Assert.Equal(3, MinOperation<int>.Combine(3, 4));

    [Fact]
    public void Combine_WithIdentity_ReturnsOtherValueUnchanged()
        => Assert.Equal(5, MinOperation<int>.Combine(MinOperation<int>.Identity, 5));
}
