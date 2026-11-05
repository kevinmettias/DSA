using DSAExperimentation.LeetCode.Solutions.AirplaneSeatAssignmentProbability;

namespace DSAExperimentation.LeetCode.Solutions.Tests.AirplaneSeatAssignmentProbability;

public sealed partial class AirplaneSeatAssignmentProbabilityReachTests
{
    [Fact]
    public void IsWithin_InputsAtTheReach_IsTrue() => Assert.True(AirplaneSeatAssignmentProbabilityReach.IsWithin(2500));

    [Fact]
    public void IsWithin_PlaneSizePastTheReach_IsFalse() => Assert.False(AirplaneSeatAssignmentProbabilityReach.IsWithin(2501));
}
