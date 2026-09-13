using DSAExperimentation.LeetCode.IsGraphBipartite;

namespace DSAExperimentation.Tests.LeetCodeCoverage.IsGraphBipartite;

// Harness only. Both strategies are IsGraphBipartiteSolution's - this file pins
// them to LeetCode's published examples plus the shapes the composed arm has to
// answer without a single connected component to walk: isolated vertices and a
// graph that is bipartite only because its components are considered separately.
public sealed class IsGraphBipartiteTests
{
    public static TheoryData<int[][], bool> Examples =>
        new()
        {
            // LC example 1: 0-1-2-0 is an odd cycle.
            { [[1, 2, 3], [0, 2], [0, 1, 3], [0, 2]], false },

            // LC example 2: a clean 4-cycle.
            { [[1, 3], [0, 2], [1, 3], [0, 2]], true },

            // A single vertex with no edges.
            { [[]], true },

            // Two isolated vertices - two components, neither with an edge.
            { [[], []], true },

            // Two disjoint edges: bipartite across more than one component.
            { [[1], [0], [3], [2]], true },

            // A triangle is the smallest odd cycle.
            { [[1, 2], [0, 2], [0, 1]], false },

            // A star: every edge crosses from the center to a leaf.
            { [[1, 2], [0], [0]], true },

            // One bipartite component and one odd-cycle component.
            { [[1], [0], [3, 4], [2, 4], [2, 3]], false },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void IsBipartiteByColorArrayDfs_LeetCodeExamples_ReturnsWhetherGraphIsTwoColorable(
        int[][] graph, bool expected) =>
        Assert.Equal(expected, IsGraphBipartiteSolution.IsBipartiteByColorArrayDfs(graph));

    [Theory]
    [MemberData(nameof(Examples))]
    public void IsBipartiteByBipartiteCheck_LeetCodeExamples_ReturnsWhetherGraphIsTwoColorable(
        int[][] graph, bool expected) =>
        Assert.Equal(expected, IsGraphBipartiteSolution.IsBipartiteByBipartiteCheck(graph));
}
