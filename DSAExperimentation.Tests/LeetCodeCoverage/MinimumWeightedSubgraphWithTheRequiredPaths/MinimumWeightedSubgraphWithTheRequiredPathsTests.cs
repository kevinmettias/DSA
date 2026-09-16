using DSAExperimentation.LeetCode.MinimumWeightedSubgraphWithTheRequiredPaths;

namespace DSAExperimentation.Tests.LeetCodeCoverage.MinimumWeightedSubgraphWithTheRequiredPaths;

// Harness only. The two-orientation graph is RequiredPathsGraph and both search
// strategies are MinimumWeightedSubgraphWithTheRequiredPathsSolution's - this file
// just pins them to LeetCode's published examples plus the unreachable and
// shared-source cases.
public sealed partial class MinimumWeightedSubgraphWithTheRequiredPathsTests
{
    public static TheoryData<SubgraphExample> Examples =>
        new()
        {
            // LeetCode example 1: both sources meet immediately - src2 = 1 reaches
            // src1 = 0 for 3, and 0 -> 2 -> 3 -> 4 -> 5 costs 6 more.
            {
                new SubgraphExample(
                    N: 6,
                    Edges: [[0, 2, 2], [0, 5, 6], [1, 0, 3], [1, 4, 5], [2, 1, 1], [2, 3, 3], [2, 3, 4], [3, 4, 2], [4, 5, 1]],
                    Src1: 0,
                    Src2: 1,
                    Dest: 5,
                    Expected: 9L)
            },

            // LeetCode example 2: dest has no incoming edge at all.
            { new SubgraphExample(N: 3, Edges: [[0, 1, 1], [2, 1, 1]], Src1: 0, Src2: 1, Dest: 2, Expected: -1L) },

            // Cheapest shared meeting vertex is 2: 0->2 (2) + 1->2 (3) + 2->3->4
            // (1+2) = 8, beating the direct 2->4 edge (5) as the shared suffix.
            {
                new SubgraphExample(
                    N: 5,
                    Edges: [[0, 2, 2], [1, 2, 3], [2, 3, 1], [2, 4, 5], [3, 4, 2]],
                    Src1: 0,
                    Src2: 1,
                    Dest: 4,
                    Expected: 8L)
            },

            // dest is reachable from src1 but src2 = 2 is isolated.
            { new SubgraphExample(N: 3, Edges: [[0, 1, 1]], Src1: 0, Src2: 2, Dest: 1, Expected: -1L) },

            // Both required paths start at the same vertex, so the answer is one
            // shortest path and nothing is paid for twice.
            { new SubgraphExample(N: 2, Edges: [[0, 1, 5]], Src1: 0, Src2: 0, Dest: 1, Expected: 5L) },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void MinimumWeightByPerNodeSearch_LeetCodeExamples_ReturnsCheapestSharedSubgraph(
        SubgraphExample example)
    {
        var actual = MinimumWeightedSubgraphWithTheRequiredPathsSolution.MinimumWeightByPerNodeSearch(
            example.N,
            example.Edges,
            new MinimumWeightedSubgraphWithTheRequiredPathsSolution.PathEndpoints(
                example.Src1, example.Src2, example.Dest));

        Assert.Equal(example.Expected, actual);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void MinimumWeightByReverseGraphDijkstra_LeetCodeExamples_ReturnsCheapestSharedSubgraph(
        SubgraphExample example)
    {
        var actual = MinimumWeightedSubgraphWithTheRequiredPathsSolution.MinimumWeightByReverseGraphDijkstra(
            example.N,
            example.Edges,
            new MinimumWeightedSubgraphWithTheRequiredPathsSolution.PathEndpoints(
                example.Src1, example.Src2, example.Dest));

        Assert.Equal(example.Expected, actual);
    }

    // One LeetCode example: the graph, the two required starting vertices, the
    // destination they both have to reach, and the cheapest shared subgraph's weight.
    // The three vertices are all `int`, so the fields name each one rather than
    // leaving a row where src1 and src2 are a transposition apart. PathEndpoints
    // itself is internal to the solution tier, so the row carries the vertices and
    // each strategy's call bundles them.
    public readonly record struct SubgraphExample(
        int N,
        int[][] Edges,
        int Src1,
        int Src2,
        int Dest,
        long Expected);
}
