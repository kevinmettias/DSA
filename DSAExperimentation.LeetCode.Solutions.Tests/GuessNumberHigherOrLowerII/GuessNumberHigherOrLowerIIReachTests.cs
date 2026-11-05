using DSAExperimentation.LeetCode.Solutions.GuessNumberHigherOrLowerII;

namespace DSAExperimentation.LeetCode.Solutions.Tests.GuessNumberHigherOrLowerII;

public sealed partial class GuessNumberHigherOrLowerIIReachTests
{
    [Fact]
    public void IsWithin_InputsAtTheReach_IsTrue() => Assert.True(GuessNumberHigherOrLowerIIReach.IsWithin(15));

    [Fact]
    public void IsWithin_HighestNumberPastTheReach_IsFalse() => Assert.False(GuessNumberHigherOrLowerIIReach.IsWithin(16));
}
