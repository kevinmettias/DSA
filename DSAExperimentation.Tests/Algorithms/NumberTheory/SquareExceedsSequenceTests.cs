using DSAExperimentation.Algorithms.NumberTheory;

namespace DSAExperimentation.Tests.Algorithms.NumberTheory;

// The virtual sequence IntegerSquareRoot.Floor binary-searches: index i reads Exceeds exactly when
// i^2 is greater than the value, so the first index that reads Exceeds is one past the floor of the
// square root.
public sealed partial class SquareExceedsSequenceTests
{
    [Fact]
    public void Get_IndexWhoseSquareExceedsTheValue_ReadsExceeds() =>
        Assert.Equal(SquareExceedsSequence.Exceeds, new SquareExceedsSequence(8, 4).Get(3));

    // Equal is not greater: 3^2 = 9 does not exceed 9, so 3 is still a candidate root.
    [Fact]
    public void Get_IndexWhoseSquareEqualsTheValue_ReadsZero() =>
        Assert.Equal(0, new SquareExceedsSequence(9, 4).Get(3));

    [Fact]
    public void Length_IsTheLengthTheSequenceWasBuiltWith() =>
        Assert.Equal(4, new SquareExceedsSequence(9, 4).Length);
}
