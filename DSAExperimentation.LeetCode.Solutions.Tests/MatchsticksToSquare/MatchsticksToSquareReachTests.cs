using DSAExperimentation.LeetCode.Solutions.MatchsticksToSquare;

namespace DSAExperimentation.LeetCode.Solutions.Tests.MatchsticksToSquare;

public sealed partial class MatchsticksToSquareReachTests
{
    [Fact]
    public void IsWithin_SticksSummingToTheReach_IsTrue() => Assert.True(MatchsticksToSquareReach.IsWithin([57, 57]));

    [Fact]
    public void IsWithin_SticksSummingPastTheReach_IsFalse() => Assert.False(MatchsticksToSquareReach.IsWithin([57, 58]));
}
