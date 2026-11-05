using DSAExperimentation.LeetCode.Solutions.RectangleOverlap;

namespace DSAExperimentation.LeetCode.Solutions.Tests.RectangleOverlap;

public sealed partial class RectangleOverlapReachTests
{
    [Fact]
    public void IsWithin_InputsAtTheReach_IsTrue() => Assert.True(RectangleOverlapReach.IsWithin([0, 0, 1500, 3000], [1500, 0, 3000, 1]));

    [Fact]
    public void IsWithin_BoxPastTheReach_IsFalse() => Assert.False(RectangleOverlapReach.IsWithin([-1_000_000_000, 0, 0, 1], [0, 0, 1_000_000_000, 1]));
}
