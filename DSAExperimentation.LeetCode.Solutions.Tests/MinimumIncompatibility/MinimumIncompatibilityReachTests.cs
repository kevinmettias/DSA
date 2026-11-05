using DSAExperimentation.LeetCode.Solutions.MinimumIncompatibility;

namespace DSAExperimentation.LeetCode.Solutions.Tests.MinimumIncompatibility;

public sealed partial class MinimumIncompatibilityReachTests
{
    [Fact]
    public void IsWithin_InputsAtTheReach_IsTrue() => Assert.True(MinimumIncompatibilityReach.IsWithin(new int[0], 2));

    [Fact]
    public void IsWithin_GroupCountPastTheReach_IsFalse() => Assert.False(MinimumIncompatibilityReach.IsWithin(new int[0], 3));
}
