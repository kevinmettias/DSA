using DSAExperimentation.LeetCode.Solutions.ShortestPathVisitingAllNodes;

namespace DSAExperimentation.LeetCode.Solutions.Tests.ShortestPathVisitingAllNodes;

public sealed partial class ShortestPathVisitingAllNodesReachTests
{
    [Fact]
    public void IsWithin_InputsAtTheReach_IsTrue() => Assert.True(ShortestPathVisitingAllNodesReach.IsWithin([new int[38]]));

    [Fact]
    public void IsWithin_GraphElementsPastTheReach_IsFalse() => Assert.False(ShortestPathVisitingAllNodesReach.IsWithin([new int[39]]));
}
