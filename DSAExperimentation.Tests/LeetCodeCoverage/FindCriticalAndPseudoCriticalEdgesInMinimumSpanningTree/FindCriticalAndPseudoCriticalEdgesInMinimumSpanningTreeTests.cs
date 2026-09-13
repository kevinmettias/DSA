using DSAExperimentation.LeetCode.FindCriticalAndPseudoCriticalEdgesInMinimumSpanningTree;

namespace DSAExperimentation.Tests.LeetCodeCoverage.FindCriticalAndPseudoCriticalEdgesInMinimumSpanningTree;

// Harness only. Both strategies live in
// FindCriticalAndPseudoCriticalEdgesInMinimumSpanningTreeSolution - this file pins
// them to LeetCode's published examples plus the shapes the three-probe classification
// has to get right: a lone bridge, a chain where every edge is critical, a triangle
// with no ties (where the heaviest edge is neither critical nor pseudo-critical), and
// two parallel edges of equal weight that are interchangeable.
//
// Both lists are reported in ascending edge index, which is the order the scan
// discovers them in.
public sealed class FindCriticalAndPseudoCriticalEdgesInMinimumSpanningTreeTests
{
    public static TheoryData<int, int[][], int[], int[]> Examples =>
        new()
        {
            // LC example 1: the two weight-1 edges are in every MST; the weight-2 and
            // weight-3 edges tie in pairs, so each is optional but usable.
            {
                5,
                [[0, 1, 1], [1, 2, 1], [2, 3, 2], [0, 3, 2], [0, 4, 3], [3, 4, 3], [1, 4, 6]],
                [0, 1],
                [2, 3, 4, 5]
            },

            // LC example 2: a four-way tie around a square - any three of the four edges
            // form an MST, so none is critical and all are pseudo-critical.
            { 4, [[0, 1, 1], [1, 2, 1], [2, 3, 1], [0, 3, 1]], [], [0, 1, 2, 3] },

            // A lone edge is the only way to connect the graph.
            { 2, [[0, 1, 5]], [0], [] },

            // A chain has no alternative anywhere, so every edge is critical.
            { 4, [[0, 1, 1], [1, 2, 2], [2, 3, 3]], [0, 1, 2], [] },

            // A triangle with distinct weights: the two light edges are forced, and the
            // heaviest is neither critical nor pseudo-critical - no MST contains it.
            { 3, [[0, 1, 1], [1, 2, 2], [0, 2, 3]], [0, 1], [] },

            // Two parallel edges of equal weight are interchangeable.
            { 2, [[0, 1, 1], [0, 1, 1]], [], [0, 1] },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void ClassifyEdgesByBfsConnectivity_LeetCodeExamples_SplitsCriticalFromPseudoCritical(
        int nodeCount, int[][] edges, int[] expectedCritical, int[] expectedPseudoCritical)
    {
        var (critical, pseudoCritical) =
            FindCriticalAndPseudoCriticalEdgesInMinimumSpanningTreeSolution
                .ClassifyEdgesByBfsConnectivity(nodeCount, edges);

        Assert.Equal(expectedCritical, critical);
        Assert.Equal(expectedPseudoCritical, pseudoCritical);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void ClassifyEdgesByDisjointSet_LeetCodeExamples_SplitsCriticalFromPseudoCritical(
        int nodeCount, int[][] edges, int[] expectedCritical, int[] expectedPseudoCritical)
    {
        var (critical, pseudoCritical) =
            FindCriticalAndPseudoCriticalEdgesInMinimumSpanningTreeSolution
                .ClassifyEdgesByDisjointSet(nodeCount, edges);

        Assert.Equal(expectedCritical, critical);
        Assert.Equal(expectedPseudoCritical, pseudoCritical);
    }
}
