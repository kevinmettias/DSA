using DSAExperimentation.LeetCode.Solutions.CountVisitedNodesInADirectedGraph;

namespace DSAExperimentation.LeetCode.Solutions.Tests.CountVisitedNodesInADirectedGraph;

public sealed partial class CountVisitedNodesInADirectedGraphPromiseTests
{
    public static TheoryData<int[], bool> Inputs =>
        new()
        {
            { [1, 2, 0, 0], true },
            { [1, 2, 3, 4, 0], true },
            { [3, 5, 5, 3, 4, 3], false },
        };

    [Theory]
    [MemberData(nameof(Inputs))]
    public void IsKeptBy_Input_HoldsWhenNoEdgeLoopsOrLeavesTheGraph(int[] edges, bool expected)
        => Assert.Equal(expected, CountVisitedNodesInADirectedGraphPromise.IsKeptBy(edges));
}
