using DSAExperimentation.LeetCode.MinimumWeightedSubgraphWithTheRequiredPaths;

namespace DSAExperimentation.Tests.LeetCodeCoverage.MinimumWeightedSubgraphWithTheRequiredPaths;

// Harness only. The two-orientation graph is RequiredPathsGraph and both search
// strategies are MinimumWeightedSubgraphWithTheRequiredPathsSolution's - this file
// just pins them to LeetCode's published examples plus the unreachable and
// shared-source cases.
public sealed class MinimumWeightedSubgraphWithTheRequiredPathsTests
{
    public static TheoryData<int, int[][], int, int, int, long> Examples =>
        new()
        {
            // LeetCode example 1: both sources meet immediately - src2 = 1 reaches
            // src1 = 0 for 3, and 0 -> 2 -> 3 -> 4 -> 5 costs 6 more.
            {
                6,
                [[0, 2, 2], [0, 5, 6], [1, 0, 3], [1, 4, 5], [2, 1, 1], [2, 3, 3], [2, 3, 4], [3, 4, 2], [4, 5, 1]],
                0, 1, 5, 9L
            },

            // LeetCode example 2: dest has no incoming edge at all.
            { 3, [[0, 1, 1], [2, 1, 1]], 0, 1, 2, -1L },

            // Cheapest shared meeting vertex is 2: 0->2 (2) + 1->2 (3) + 2->3->4
            // (1+2) = 8, beating the direct 2->4 edge (5) as the shared suffix.
            { 5, [[0, 2, 2], [1, 2, 3], [2, 3, 1], [2, 4, 5], [3, 4, 2]], 0, 1, 4, 8L },

            // dest is reachable from src1 but src2 = 2 is isolated.
            { 3, [[0, 1, 1]], 0, 2, 1, -1L },

            // Both required paths start at the same vertex, so the answer is one
            // shortest path and nothing is paid for twice.
            { 2, [[0, 1, 5]], 0, 0, 1, 5L },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void MinimumWeightByPerNodeSearch_LeetCodeExamples_ReturnsCheapestSharedSubgraph(
        int n, int[][] edges, int src1, int src2, int dest, long expected) =>
        Assert.Equal(
            expected,
            MinimumWeightedSubgraphWithTheRequiredPathsSolution.MinimumWeightByPerNodeSearch(
                n, edges, src1, src2, dest));

    [Theory]
    [MemberData(nameof(Examples))]
    public void MinimumWeightByReverseGraphDijkstra_LeetCodeExamples_ReturnsCheapestSharedSubgraph(
        int n, int[][] edges, int src1, int src2, int dest, long expected) =>
        Assert.Equal(
            expected,
            MinimumWeightedSubgraphWithTheRequiredPathsSolution.MinimumWeightByReverseGraphDijkstra(
                n, edges, src1, src2, dest));
}
