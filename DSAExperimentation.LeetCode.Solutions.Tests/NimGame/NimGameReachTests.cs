using DSAExperimentation.LeetCode.Solutions.NimGame;

namespace DSAExperimentation.LeetCode.Solutions.Tests.NimGame;

public sealed partial class NimGameReachTests
{
    [Fact]
    public void IsWithin_InputsAtTheReach_IsTrue() => Assert.True(NimGameReach.IsWithin(1000));

    [Fact]
    public void IsWithin_StoneCountPastTheReach_IsFalse() => Assert.False(NimGameReach.IsWithin(1001));
}
