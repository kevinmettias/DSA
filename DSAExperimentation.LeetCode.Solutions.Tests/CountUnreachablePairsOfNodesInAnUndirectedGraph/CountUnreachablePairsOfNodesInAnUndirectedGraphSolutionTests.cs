using DSAExperimentation.LeetCode.Solutions.CountUnreachablePairsOfNodesInAnUndirectedGraph;

namespace DSAExperimentation.LeetCode.Solutions.Tests.CountUnreachablePairsOfNodesInAnUndirectedGraph;

// Harness only: both strategies live in
// CountUnreachablePairsOfNodesInAnUndirectedGraphSolution and are asserted against
// the same cases - LeetCode's two published ones plus the edge-free graph (every
// pair unreachable), a single fully connected component (none), and a graph whose
// components have different sizes including an isolated node, which is where a
// size tally that misses singletons goes wrong.
public sealed partial class CountUnreachablePairsOfNodesInAnUndirectedGraphSolutionTests
{
    public static TheoryData<int, int[][], long> Cases =>
        new()
        {
            { 7, [[0, 2], [0, 1], [1, 2], [3, 4], [4, 5], [5, 6]], 12L },
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void CountPairsByDepthFirstFloodFill_Cases_ReturnsPairsSplitAcrossComponents(
        int nodeCount, int[][] edges, long expected)
    {
        var pairs = CountUnreachablePairsOfNodesInAnUndirectedGraphSolution.CountPairsByDepthFirstFloodFill(nodeCount, edges);
        Assert.Equal(expected, pairs);
    }

    [Theory]
    [MemberData(nameof(Cases))]
    public void CountPairsByDisjointSet_Cases_ReturnsPairsSplitAcrossComponents(
        int nodeCount, int[][] edges, long expected)
    {
        var pairs = CountUnreachablePairsOfNodesInAnUndirectedGraphSolution.CountPairsByDisjointSet(nodeCount, edges);
        Assert.Equal(expected, pairs);
    }
}
