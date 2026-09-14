using DSAExperimentation.LeetCode.CountUnreachablePairsOfNodesInAnUndirectedGraph;

namespace DSAExperimentation.Tests.LeetCodeCoverage.CountUnreachablePairsOfNodesInAnUndirectedGraph;

// Harness only: both strategies live in
// CountUnreachablePairsOfNodesInAnUndirectedGraphSolution and are asserted against
// the same examples - LeetCode's two published ones plus the edge-free graph (every
// pair unreachable), a single fully connected component (none), and a graph whose
// components have different sizes including an isolated node, which is where a
// size tally that misses singletons goes wrong.
public sealed class CountUnreachablePairsOfNodesInAnUndirectedGraphTests
{
    public static TheoryData<int, int[][], long> Examples =>
        new()
        {
            { 3, [[0, 1], [0, 2], [1, 2]], 0L },
            { 7, [[0, 2], [0, 5], [2, 4], [1, 6], [5, 4]], 14L },
            { 7, [[0, 2], [0, 1], [1, 2], [3, 4], [4, 5], [5, 6]], 12L },
            { 4, [], 6L },
            { 1, [], 0L },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void CountPairsByDepthFirstFloodFill_LeetCodeExamples_ReturnsPairsSplitAcrossComponents(
        int n, int[][] edges, long expected) =>
        Assert.Equal(
            expected,
            CountUnreachablePairsOfNodesInAnUndirectedGraphSolution.CountPairsByDepthFirstFloodFill(n, edges));

    [Theory]
    [MemberData(nameof(Examples))]
    public void CountPairsByDisjointSet_LeetCodeExamples_ReturnsPairsSplitAcrossComponents(
        int n, int[][] edges, long expected) =>
        Assert.Equal(
            expected,
            CountUnreachablePairsOfNodesInAnUndirectedGraphSolution.CountPairsByDisjointSet(n, edges));
}
