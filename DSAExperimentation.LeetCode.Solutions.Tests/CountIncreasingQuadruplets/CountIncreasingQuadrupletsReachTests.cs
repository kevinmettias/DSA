using DSAExperimentation.LeetCode.Solutions.CountIncreasingQuadruplets;

namespace DSAExperimentation.LeetCode.Solutions.Tests.CountIncreasingQuadruplets;

public sealed partial class CountIncreasingQuadrupletsReachTests
{
    [Fact]
    public void IsWithin_InputsAtTheReach_IsTrue() => Assert.True(CountIncreasingQuadrupletsReach.IsWithin(new int[251]));

    [Fact]
    public void IsWithin_NumsLengthPastTheReach_IsFalse() => Assert.False(CountIncreasingQuadrupletsReach.IsWithin(new int[252]));
}
