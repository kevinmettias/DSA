using DSAExperimentation.LeetCode.RedundantConnectionII;

namespace DSAExperimentation.Tests.LeetCodeCoverage.RedundantConnectionII;

// LeetCode 685. Redundant Connection II: the directed-graph sibling of LC 684
// (Redundant Connection). Harness only: both strategies are
// RedundantConnectionIISolution's - this file pins them to LeetCode's published
// examples, including the two-parents-but-still-cycles case that distinguishes
// which of the two conflicting edges is the real answer.
public sealed class RedundantConnectionIITests
{
    public static TheoryData<int[][], int[]> Examples =>
        new()
        {
            { [[1, 2], [1, 3], [2, 3]], [2, 3] },
            { [[1, 2], [2, 3], [3, 4], [4, 1], [1, 5]], [4, 1] },
            { [[2, 1], [3, 1], [4, 2], [1, 4]], [2, 1] },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void FindRedundantEdgeByDisjointSet_LeetCodeExamples_ReturnsTheRedundantEdge(
        int[][] edges, int[] expected) =>
        Assert.Equal(expected, RedundantConnectionIISolution.FindRedundantEdgeByDisjointSet(edges));

    [Theory]
    [MemberData(nameof(Examples))]
    public void FindRedundantEdgeByRemovalScan_LeetCodeExamples_ReturnsTheRedundantEdge(
        int[][] edges, int[] expected) =>
        Assert.Equal(expected, RedundantConnectionIISolution.FindRedundantEdgeByRemovalScan(edges));
}
