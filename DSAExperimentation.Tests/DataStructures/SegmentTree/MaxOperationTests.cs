using DSAExperimentation.DataStructures.SegmentTree;

namespace DSAExperimentation.Tests.DataStructures.SegmentTree;

public sealed partial class MaxOperationTests
{
    [Fact]
    public void Identity_IsMinValue() => Assert.Equal(int.MinValue, MaxOperation<int>.Identity);

    [Fact]
    public void Combine_ReturnsLargerValue() => Assert.Equal(4, MaxOperation<int>.Combine(3, 4));

    [Fact]
    public void Combine_WithIdentity_ReturnsOtherValueUnchanged()
        => Assert.Equal(5, MaxOperation<int>.Combine(MaxOperation<int>.Identity, 5));
}
