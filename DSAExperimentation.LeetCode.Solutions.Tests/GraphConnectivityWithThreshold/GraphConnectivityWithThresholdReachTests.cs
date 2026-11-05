using DSAExperimentation.LeetCode.Solutions.GraphConnectivityWithThreshold;

namespace DSAExperimentation.LeetCode.Solutions.Tests.GraphConnectivityWithThreshold;

public sealed partial class GraphConnectivityWithThresholdReachTests
{
    [Fact]
    public void IsWithin_InputsAtTheReach_IsTrue() => Assert.True(GraphConnectivityWithThresholdReach.IsWithin(8000, 1, Enumerable.Repeat(Array.Empty<int>(), 0).ToArray()));

    [Fact]
    public void IsWithin_CityCountPastTheReach_IsFalse() => Assert.False(GraphConnectivityWithThresholdReach.IsWithin(8001, 1, Enumerable.Repeat(Array.Empty<int>(), 0).ToArray()));
}
