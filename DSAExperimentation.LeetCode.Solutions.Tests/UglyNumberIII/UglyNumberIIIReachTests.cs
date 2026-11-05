using DSAExperimentation.LeetCode.Solutions.UglyNumberIII;

namespace DSAExperimentation.LeetCode.Solutions.Tests.UglyNumberIII;

public sealed partial class UglyNumberIIIReachTests
{
    [Fact]
    public void IsWithin_InputsAtTheReach_IsTrue() => Assert.True(UglyNumberIIIReach.IsWithin(500_000, 123, 1, 1));

    [Fact]
    public void IsWithin_RankPastTheReach_IsFalse() => Assert.False(UglyNumberIIIReach.IsWithin(500_001, 123, 1, 1));

    [Fact]
    public void IsWithin_FirstFactorPastTheReach_IsFalse() => Assert.False(UglyNumberIIIReach.IsWithin(500_000, 124, 1, 1));
}
