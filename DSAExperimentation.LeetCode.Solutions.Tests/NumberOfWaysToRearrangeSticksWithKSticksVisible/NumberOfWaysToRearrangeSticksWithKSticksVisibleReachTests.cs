using DSAExperimentation.LeetCode.Solutions.NumberOfWaysToRearrangeSticksWithKSticksVisible;

namespace DSAExperimentation.LeetCode.Solutions.Tests.NumberOfWaysToRearrangeSticksWithKSticksVisible;

public sealed partial class NumberOfWaysToRearrangeSticksWithKSticksVisibleReachTests
{
    [Fact]
    public void IsWithin_InputsAtTheReach_IsTrue() => Assert.True(NumberOfWaysToRearrangeSticksWithKSticksVisibleReach.IsWithin(10, 1));

    [Fact]
    public void IsWithin_StickCountPastTheReach_IsFalse() => Assert.False(NumberOfWaysToRearrangeSticksWithKSticksVisibleReach.IsWithin(11, 1));
}
