using DSAExperimentation.LeetCode.Solutions.CountPrimes;

namespace DSAExperimentation.LeetCode.Solutions.Tests.CountPrimes;

public sealed partial class CountPrimesReachTests
{
    [Fact]
    public void IsWithin_InputsAtTheReach_IsTrue() => Assert.True(CountPrimesReach.IsWithin(789_654));

    [Fact]
    public void IsWithin_LimitPastTheReach_IsFalse() => Assert.False(CountPrimesReach.IsWithin(789_655));
}
