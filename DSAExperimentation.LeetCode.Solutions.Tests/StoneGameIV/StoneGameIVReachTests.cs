using DSAExperimentation.LeetCode.Solutions.StoneGameIV;

namespace DSAExperimentation.LeetCode.Solutions.Tests.StoneGameIV;

public sealed partial class StoneGameIVReachTests
{
    [Fact]
    public void IsWithin_InputsAtTheReach_IsTrue() => Assert.True(StoneGameIVReach.IsWithin(55));

    [Fact]
    public void IsWithin_StoneCountPastTheReach_IsFalse() => Assert.False(StoneGameIVReach.IsWithin(56));
}
