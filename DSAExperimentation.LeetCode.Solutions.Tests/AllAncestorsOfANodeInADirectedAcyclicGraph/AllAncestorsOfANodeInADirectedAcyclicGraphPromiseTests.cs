using DSAExperimentation.LeetCode.Solutions.AllAncestorsOfANodeInADirectedAcyclicGraph;

namespace DSAExperimentation.LeetCode.Solutions.Tests.AllAncestorsOfANodeInADirectedAcyclicGraph;

public sealed partial class AllAncestorsOfANodeInADirectedAcyclicGraphPromiseTests
{
    public static TheoryData<int, int[][], bool> Inputs =>
        new()
        {
            { 5, [[0, 1], [0, 2], [1, 3], [2, 3], [3, 4]], true },
            { 3, [], true },
            { 3, [[0, 1], [1, 2], [2, 0]], false },
        };

    [Theory]
    [MemberData(nameof(Inputs))]
    public void IsKeptBy_Input_HoldsForAnAcyclicGraph(int nodeCount, int[][] edges, bool expected)
        => Assert.Equal(expected, AllAncestorsOfANodeInADirectedAcyclicGraphPromise.IsKeptBy(nodeCount, edges));
}
