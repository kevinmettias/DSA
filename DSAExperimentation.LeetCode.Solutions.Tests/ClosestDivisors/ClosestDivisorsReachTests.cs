using DSAExperimentation.LeetCode.Solutions.ClosestDivisors;

namespace DSAExperimentation.LeetCode.Solutions.Tests.ClosestDivisors;

public sealed partial class ClosestDivisorsReachTests
{
    [Fact]
    public void IsWithin_InputsAtTheReach_IsTrue() => Assert.True(ClosestDivisorsReach.IsWithin(42_424_242));

    [Fact]
    public void IsWithin_NumberPastTheReach_IsFalse() => Assert.False(ClosestDivisorsReach.IsWithin(42_424_243));
}
