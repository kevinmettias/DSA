using DSAExperimentation.LeetCode.Solutions.NumberOfWaysToReachAPositionAfterExactlyKSteps;

namespace DSAExperimentation.LeetCode.Solutions.Tests.NumberOfWaysToReachAPositionAfterExactlyKSteps;

public sealed partial class NumberOfWaysToReachAPositionAfterExactlyKStepsReachTests
{
    [Fact]
    public void IsWithin_InputsAtTheReach_IsTrue() => Assert.True(NumberOfWaysToReachAPositionAfterExactlyKStepsReach.IsWithin(1, 1, 15));

    [Fact]
    public void IsWithin_StepCountPastTheReach_IsFalse() => Assert.False(NumberOfWaysToReachAPositionAfterExactlyKStepsReach.IsWithin(1, 1, 16));
}
