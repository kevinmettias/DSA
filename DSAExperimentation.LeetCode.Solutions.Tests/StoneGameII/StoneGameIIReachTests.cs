using DSAExperimentation.LeetCode.Solutions.StoneGameII;

namespace DSAExperimentation.LeetCode.Solutions.Tests.StoneGameII;

public sealed partial class StoneGameIIReachTests
{
    [Fact]
    public void IsWithin_InputsAtTheReach_IsTrue() => Assert.True(StoneGameIIReach.IsWithin(new int[25]));

    [Fact]
    public void IsWithin_PilesLengthPastTheReach_IsFalse() => Assert.False(StoneGameIIReach.IsWithin(new int[26]));
}
