using DSAExperimentation.DataStructures.RangeFenwickTree;

namespace DSAExperimentation.Tests.DataStructures.RangeFenwickTree;

public sealed partial class ScaledSumOperationTests
{
    [Fact]
    public void Identity_IsZero() => Assert.Equal(0, ScaledSumOperation<int>.Identity);

    [Fact]
    public void Combine_AddsBothValues() => Assert.Equal(7, ScaledSumOperation<int>.Combine(3, 4));

    [Fact]
    public void Scale_MultipliesValueByCount() => Assert.Equal(20, ScaledSumOperation<int>.Scale(4, 5));

    [Fact]
    public void Scale_ByZero_ReturnsIdentity() => Assert.Equal(ScaledSumOperation<int>.Identity, ScaledSumOperation<int>.Scale(4, 0));

    [Fact]
    public void Invert_NegatesValue() => Assert.Equal(-5, ScaledSumOperation<int>.Invert(5));
}
