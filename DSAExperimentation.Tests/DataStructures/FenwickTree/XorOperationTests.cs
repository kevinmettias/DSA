using DSAExperimentation.DataStructures.FenwickTree;

namespace DSAExperimentation.Tests.DataStructures.FenwickTree;

public sealed partial class XorOperationTests
{
    [Fact]
    public void Identity_IsZero() => Assert.Equal(0, XorOperation<int>.Identity);

    [Fact]
    public void Combine_XorsBothValues() => Assert.Equal(6, XorOperation<int>.Combine(5, 3));

    [Fact]
    public void Invert_IsSelfInverse() => Assert.Equal(5, XorOperation<int>.Invert(5));

    [Fact]
    public void Combine_WithInvertOfSameValue_ReturnsIdentity()
        => Assert.Equal(XorOperation<int>.Identity, XorOperation<int>.Combine(5, XorOperation<int>.Invert(5)));
}
