using DSAExperimentation.LeetCode.Solutions.KthSmallestInLexicographicalOrder;

namespace DSAExperimentation.LeetCode.Solutions.Tests.KthSmallestInLexicographicalOrder;

public sealed partial class KthSmallestInLexicographicalOrderReachTests
{
    [Fact]
    public void IsWithin_InputsAtTheReach_IsTrue() => Assert.True(KthSmallestInLexicographicalOrderReach.IsWithin(10_000, 1));

    [Fact]
    public void IsWithin_UpperBoundPastTheReach_IsFalse() => Assert.False(KthSmallestInLexicographicalOrderReach.IsWithin(10_001, 1));
}
