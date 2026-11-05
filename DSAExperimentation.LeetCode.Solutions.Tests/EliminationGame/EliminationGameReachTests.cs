using DSAExperimentation.LeetCode.Solutions.EliminationGame;

namespace DSAExperimentation.LeetCode.Solutions.Tests.EliminationGame;

public sealed partial class EliminationGameReachTests
{
    [Fact]
    public void IsWithin_InputsAtTheReach_IsTrue() => Assert.True(EliminationGameReach.IsWithin(7_812_500));

    [Fact]
    public void IsWithin_NumberCountPastTheReach_IsFalse() => Assert.False(EliminationGameReach.IsWithin(7_812_501));
}
