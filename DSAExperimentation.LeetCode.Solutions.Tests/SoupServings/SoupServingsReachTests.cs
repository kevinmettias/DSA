using DSAExperimentation.LeetCode.Solutions.SoupServings;

namespace DSAExperimentation.LeetCode.Solutions.Tests.SoupServings;

public sealed partial class SoupServingsReachTests
{
    [Fact]
    public void IsWithin_InputsAtTheReach_IsTrue() => Assert.True(SoupServingsReach.IsWithin(625));

    [Fact]
    public void IsWithin_MillilitersPastTheReach_IsFalse() => Assert.False(SoupServingsReach.IsWithin(626));
}
