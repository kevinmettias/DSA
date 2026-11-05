using DSAExperimentation.LeetCode.Solutions.SumGame;

namespace DSAExperimentation.LeetCode.Solutions.Tests.SumGame;

public sealed partial class SumGameReachTests
{
    [Fact]
    public void IsWithin_InputsAtTheReach_IsTrue() => Assert.True(SumGameReach.IsWithin(new string('a', 9)));

    [Fact]
    public void IsWithin_NumLengthPastTheReach_IsFalse() => Assert.False(SumGameReach.IsWithin(new string('a', 10)));
}
