using DSAExperimentation.DataStructures.FenwickTree;

namespace DSAExperimentation.Tests.DataStructures.FenwickTree;

public sealed partial class SumOperationTests
{
    [Fact]
    public void Identity_IsZero() => Assert.Equal(0, SumOperation<int>.Identity);

    [Fact]
    public void Combine_AddsBothValues() => Assert.Equal(7, SumOperation<int>.Combine(3, 4));

    [Fact]
    public void Invert_NegatesValue() => Assert.Equal(-5, SumOperation<int>.Invert(5));

    [Fact]
    public void Combine_WithInvertOfSameValue_ReturnsIdentity()
        => Assert.Equal(SumOperation<int>.Identity, SumOperation<int>.Combine(5, SumOperation<int>.Invert(5)));
}
