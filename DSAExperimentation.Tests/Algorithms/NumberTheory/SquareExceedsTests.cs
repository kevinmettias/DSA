using DSAExperimentation.Algorithms.NumberTheory;

namespace DSAExperimentation.Tests.Algorithms.NumberTheory;

// The rule IntegerSquareRoot.Floor bisects: a candidate holds exactly when its square is greater than
// the value, so the first candidate that holds is one past the floor of the square root.
public sealed partial class SquareExceedsTests
{
    [Fact]
    public void IsSatisfiedBy_CandidateWhoseSquareExceedsTheValue_ReturnsTrue() =>
        Assert.True(new SquareExceeds(8).IsSatisfiedBy(3));

    // Equal is not greater: 3^2 = 9 does not exceed 9, so 3 is still a candidate root.
    [Fact]
    public void IsSatisfiedBy_CandidateWhoseSquareEqualsTheValue_ReturnsFalse() =>
        Assert.False(new SquareExceeds(9).IsSatisfiedBy(3));

    // 46,341^2 is past int.MaxValue; the square is taken in long, so it still compares correctly.
    [Fact]
    public void IsSatisfiedBy_CandidateWhoseSquareLeavesInt_ReturnsTrue() =>
        Assert.True(new SquareExceeds(int.MaxValue).IsSatisfiedBy(46_341));
}
