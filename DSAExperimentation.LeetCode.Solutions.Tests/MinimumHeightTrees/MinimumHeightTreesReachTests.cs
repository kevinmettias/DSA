using DSAExperimentation.LeetCode.Solutions.MinimumHeightTrees;

namespace DSAExperimentation.LeetCode.Solutions.Tests.MinimumHeightTrees;

public sealed partial class MinimumHeightTreesReachTests
{
    [Fact]
    public void IsWithin_InputsAtTheReach_IsTrue() => Assert.True(MinimumHeightTreesReach.IsWithin(1, Enumerable.Repeat(Array.Empty<int>(), 11).ToArray()));

    [Fact]
    public void IsWithin_EdgesLengthPastTheReach_IsFalse() => Assert.False(MinimumHeightTreesReach.IsWithin(1, Enumerable.Repeat(Array.Empty<int>(), 12).ToArray()));
}
