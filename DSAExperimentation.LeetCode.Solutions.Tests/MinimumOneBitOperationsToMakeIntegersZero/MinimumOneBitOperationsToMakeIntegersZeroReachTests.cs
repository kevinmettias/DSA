using DSAExperimentation.LeetCode.Solutions.MinimumOneBitOperationsToMakeIntegersZero;

namespace DSAExperimentation.LeetCode.Solutions.Tests.MinimumOneBitOperationsToMakeIntegersZero;

public sealed partial class MinimumOneBitOperationsToMakeIntegersZeroReachTests
{
    [Fact]
    public void IsWithin_InputsAtTheReach_IsTrue() => Assert.True(MinimumOneBitOperationsToMakeIntegersZeroReach.IsWithin(1_000_000));

    [Fact]
    public void IsWithin_TargetPastTheReach_IsFalse() => Assert.False(MinimumOneBitOperationsToMakeIntegersZeroReach.IsWithin(1_000_001));
}
