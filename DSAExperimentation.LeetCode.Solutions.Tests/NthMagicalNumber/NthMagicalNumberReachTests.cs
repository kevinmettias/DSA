using DSAExperimentation.LeetCode.Solutions.NthMagicalNumber;

namespace DSAExperimentation.LeetCode.Solutions.Tests.NthMagicalNumber;

public sealed partial class NthMagicalNumberReachTests
{
    [Fact]
    public void IsWithin_InputsAtTheReach_IsTrue() => Assert.True(NthMagicalNumberReach.IsWithin(1000, 1, 1));

    [Fact]
    public void IsWithin_RankPastTheReach_IsFalse() => Assert.False(NthMagicalNumberReach.IsWithin(1001, 1, 1));
}
