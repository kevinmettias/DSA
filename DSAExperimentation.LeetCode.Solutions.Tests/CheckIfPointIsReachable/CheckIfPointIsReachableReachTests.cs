using DSAExperimentation.LeetCode.Solutions.CheckIfPointIsReachable;

namespace DSAExperimentation.LeetCode.Solutions.Tests.CheckIfPointIsReachable;

public sealed partial class CheckIfPointIsReachableReachTests
{
    [Fact]
    public void IsWithin_InputsAtTheReach_IsTrue() => Assert.True(CheckIfPointIsReachableReach.IsWithin(1023, 513));

    [Fact]
    public void IsWithin_TargetXPastTheReach_IsFalse() => Assert.False(CheckIfPointIsReachableReach.IsWithin(1024, 513));

    [Fact]
    public void IsWithin_TargetYPastTheReach_IsFalse() => Assert.False(CheckIfPointIsReachableReach.IsWithin(1023, 514));
}
