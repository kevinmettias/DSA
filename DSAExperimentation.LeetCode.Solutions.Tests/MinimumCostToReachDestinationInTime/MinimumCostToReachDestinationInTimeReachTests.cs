using DSAExperimentation.LeetCode.Solutions.MinimumCostToReachDestinationInTime;

namespace DSAExperimentation.LeetCode.Solutions.Tests.MinimumCostToReachDestinationInTime;

public sealed partial class MinimumCostToReachDestinationInTimeReachTests
{
    [Fact]
    public void IsWithin_InputsAtTheReach_IsTrue() => Assert.True(MinimumCostToReachDestinationInTimeReach.IsWithin(1, Enumerable.Repeat(Array.Empty<int>(), 4).ToArray(), new int[0]));

    [Fact]
    public void IsWithin_EdgesLengthPastTheReach_IsFalse() => Assert.False(MinimumCostToReachDestinationInTimeReach.IsWithin(1, Enumerable.Repeat(Array.Empty<int>(), 5).ToArray(), new int[0]));
}
