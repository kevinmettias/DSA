using DSAExperimentation.LeetCode.Solutions.CountTheNumberOfCompleteComponents;

namespace DSAExperimentation.LeetCode.Solutions.Tests.CountTheNumberOfCompleteComponents;

// Harness only. Both strategies are CountTheNumberOfCompleteComponentsSolution's -
// the O(k^2) pairwise adjacency scan that reads "complete" literally, and the
// DisjointSet tally that replaces it with the k*(k-1)/2 identity - and this file
// pins both to the degenerate shapes the
// identity has to get right: an isolated node (k = 1 needs no edges) and a cycle
// that is connected without being complete.
public sealed partial class CountTheNumberOfCompleteComponentsSolutionTests
{
    public static TheoryData<int, int[][], int> Cases =>
        new()
        {
            { 3, [], 3 },
            { 5, [[0, 1], [1, 2], [0, 2], [3, 4]], 2 },
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void CountCompleteComponentsByAdjacencySetScan_Cases_CountsFullyConnectedComponents(
        int nodeCount, int[][] edges, int expected)
    {
        var actual = CountTheNumberOfCompleteComponentsSolution.CountCompleteComponentsByAdjacencySetScan(nodeCount, edges);
        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Cases))]
    public void CountCompleteComponentsByDisjointSetTally_Cases_CountsFullyConnectedComponents(
        int nodeCount, int[][] edges, int expected)
    {
        var actual = CountTheNumberOfCompleteComponentsSolution.CountCompleteComponentsByDisjointSetTally(nodeCount, edges);
        Assert.Equal(expected, actual);
    }
}
