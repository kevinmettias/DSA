using DSAExperimentation.LeetCode.Solutions.CheckIfItIsAGoodArray;

namespace DSAExperimentation.LeetCode.Solutions.Tests.CheckIfItIsAGoodArray;

public sealed partial class CheckIfItIsAGoodArrayReachTests
{
    [Fact]
    public void IsWithin_ElementsSummingToTheReach_IsTrue() => Assert.True(CheckIfItIsAGoodArrayReach.IsWithin([937_500_000, 1_000_000_000]));

    [Fact]
    public void IsWithin_ElementsSummingPastTheReach_IsFalse() => Assert.False(CheckIfItIsAGoodArrayReach.IsWithin([937_500_001, 1_000_000_000]));
}
