using DSAExperimentation.DataStructures.SegmentTree;

namespace DSAExperimentation.Tests.DataStructures.SegmentTree;

public sealed partial class SumOperationTests
{
    [Fact]
    public void Identity_IsZero() => Assert.Equal(0, SumOperation<int>.Identity);

    [Fact]
    public void Combine_AddsBothValues() => Assert.Equal(7, SumOperation<int>.Combine(3, 4));

    [Fact]
    public void Combine_WithIdentity_ReturnsOtherValueUnchanged()
        => Assert.Equal(5, SumOperation<int>.Combine(SumOperation<int>.Identity, 5));
}
