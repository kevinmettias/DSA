using DSAExperimentation.LeetCode.Solutions.CanIWin;

namespace DSAExperimentation.LeetCode.Solutions.Tests.CanIWin;

public sealed partial class CanIWinReachTests
{
    [Fact]
    public void IsWithin_InputsAtTheReach_IsTrue() => Assert.True(CanIWinReach.IsWithin(1, 75));

    [Fact]
    public void IsWithin_DesiredTotalPastTheReach_IsFalse() => Assert.False(CanIWinReach.IsWithin(1, 76));
}
