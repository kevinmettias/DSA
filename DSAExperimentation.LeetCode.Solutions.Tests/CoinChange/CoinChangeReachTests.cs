using DSAExperimentation.LeetCode.Solutions.CoinChange;

namespace DSAExperimentation.LeetCode.Solutions.Tests.CoinChange;

public sealed partial class CoinChangeReachTests
{
    [Fact]
    public void IsWithin_InputsAtTheReach_IsTrue() => Assert.True(CoinChangeReach.IsWithin(new int[0], 6));

    [Fact]
    public void IsWithin_AmountPastTheReach_IsFalse() => Assert.False(CoinChangeReach.IsWithin(new int[0], 7));
}
