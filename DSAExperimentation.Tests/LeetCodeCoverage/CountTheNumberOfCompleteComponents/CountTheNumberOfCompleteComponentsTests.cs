using DSAExperimentation.LeetCode.CountTheNumberOfCompleteComponents;

namespace DSAExperimentation.Tests.LeetCodeCoverage.CountTheNumberOfCompleteComponents;

// Harness only. Both strategies are CountTheNumberOfCompleteComponentsSolution's -
// the O(k^2) pairwise adjacency scan that reads "complete" literally, and the
// DisjointSet tally that replaces it with the k*(k-1)/2 identity - and this file
// pins both to LeetCode's published examples plus the degenerate shapes the
// identity has to get right: an isolated node (k = 1 needs no edges) and a cycle
// that is connected without being complete.
public sealed partial class CountTheNumberOfCompleteComponentsTests
{
    public static TheoryData<int, int[][], int> Examples =>
        new()
        {
            { 6, [[0, 1], [0, 2], [1, 2], [3, 4]], 3 },
            { 6, [[0, 1], [0, 2], [1, 2], [3, 4], [3, 5]], 1 },
            { 1, [], 1 },
            { 3, [], 3 },
            { 4, [[0, 1], [2, 3]], 2 },
            { 4, [[0, 1], [1, 2], [2, 3], [3, 0]], 0 },
            { 5, [[0, 1], [1, 2], [0, 2], [3, 4]], 2 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void CountCompleteComponentsByAdjacencySetScan_LeetCodeExamples_CountsFullyConnectedComponents(
        int nodeCount, int[][] edges, int expected)
    {
        var actual = CountTheNumberOfCompleteComponentsSolution.CountCompleteComponentsByAdjacencySetScan(nodeCount, edges);
        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void CountCompleteComponentsByDisjointSetTally_LeetCodeExamples_CountsFullyConnectedComponents(
        int nodeCount, int[][] edges, int expected)
    {
        var actual = CountTheNumberOfCompleteComponentsSolution.CountCompleteComponentsByDisjointSetTally(nodeCount, edges);
        Assert.Equal(expected, actual);
    }
}
