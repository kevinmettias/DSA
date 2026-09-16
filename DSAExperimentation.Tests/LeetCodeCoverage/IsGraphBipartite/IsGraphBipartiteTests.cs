using DSAExperimentation.LeetCode.IsGraphBipartite;

namespace DSAExperimentation.Tests.LeetCodeCoverage.IsGraphBipartite;

// Harness only. Both strategies are IsGraphBipartiteSolution's - this file pins
// them to LeetCode's published examples plus the shapes the composed arm has to
// answer without a single connected component to walk: isolated vertices and a
// graph that is bipartite only because its components are considered separately.
public sealed class IsGraphBipartiteTests
{
    public static TheoryData<BipartiteExample> Examples =>
        new()
        {
            // LC example 1: 0-1-2-0 is an odd cycle.
            { new BipartiteExample(Graph: [[1, 2, 3], [0, 2], [0, 1, 3], [0, 2]], Expected: false) },

            // LC example 2: a clean 4-cycle.
            { new BipartiteExample(Graph: [[1, 3], [0, 2], [1, 3], [0, 2]], Expected: true) },

            // A single vertex with no edges.
            { new BipartiteExample(Graph: [[]], Expected: true) },

            // Two isolated vertices - two components, neither with an edge.
            { new BipartiteExample(Graph: [[], []], Expected: true) },

            // Two disjoint edges: bipartite across more than one component.
            { new BipartiteExample(Graph: [[1], [0], [3], [2]], Expected: true) },

            // A triangle is the smallest odd cycle.
            { new BipartiteExample(Graph: [[1, 2], [0, 2], [0, 1]], Expected: false) },

            // A star: every edge crosses from the center to a leaf.
            { new BipartiteExample(Graph: [[1, 2], [0], [0]], Expected: true) },

            // One bipartite component and one odd-cycle component.
            { new BipartiteExample(Graph: [[1], [0], [3, 4], [2, 4], [2, 3]], Expected: false) },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void IsBipartiteByColorArrayDfs_LeetCodeExamples_ReturnsWhetherGraphIsTwoColorable(
        BipartiteExample example)
    {
        var actual = IsGraphBipartiteSolution.IsBipartiteByColorArrayDfs(example.Graph);

        Assert.Equal(example.Expected, actual);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void IsBipartiteByBipartiteCheck_LeetCodeExamples_ReturnsWhetherGraphIsTwoColorable(
        BipartiteExample example)
    {
        var actual = IsGraphBipartiteSolution.IsBipartiteByBipartiteCheck(example.Graph);

        Assert.Equal(example.Expected, actual);
    }

    // One LeetCode example: the adjacency list, and whether the graph is two-colorable.
    // The `bool` is the expected answer rather than a mode, so the row names it instead
    // of leaving a bare `true` in a position the reader has to decode.
    public readonly record struct BipartiteExample(int[][] Graph, bool Expected);
}
