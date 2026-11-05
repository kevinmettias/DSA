using DSAExperimentation.LeetCode.Solutions.StoneGame;

namespace DSAExperimentation.LeetCode.Solutions.Tests.StoneGame;

public sealed partial class StoneGameReachTests
{
    [Fact]
    public void IsWithin_InputsAtTheReach_IsTrue() => Assert.True(StoneGameReach.IsWithin(new int[24]));

    [Fact]
    public void IsWithin_PilesLengthPastTheReach_IsFalse() => Assert.False(StoneGameReach.IsWithin(new int[25]));
}
