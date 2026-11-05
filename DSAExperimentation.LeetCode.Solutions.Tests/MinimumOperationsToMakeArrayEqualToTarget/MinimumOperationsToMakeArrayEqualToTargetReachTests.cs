using DSAExperimentation.LeetCode.Solutions.MinimumOperationsToMakeArrayEqualToTarget;

namespace DSAExperimentation.LeetCode.Solutions.Tests.MinimumOperationsToMakeArrayEqualToTarget;

public sealed partial class MinimumOperationsToMakeArrayEqualToTargetReachTests
{
    [Fact]
    public void IsWithin_ElementsAtTheReachFromTheirTargets_IsTrue() =>
        Assert.True(MinimumOperationsToMakeArrayEqualToTargetReach.IsWithin([1, 1_000_000], [1_000_000, 1]));

    [Fact]
    public void IsWithin_AnElementPastTheReachFromItsTarget_IsFalse() =>
        Assert.False(MinimumOperationsToMakeArrayEqualToTargetReach.IsWithin([1, 1], [1, 1_000_001]));
}
