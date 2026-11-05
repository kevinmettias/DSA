using DSAExperimentation.LeetCode.Solutions.IsGraphBipartite;

namespace DSAExperimentation.LeetCode.Solutions.Tests.IsGraphBipartite;

// Harness only. Both strategies are IsGraphBipartiteSolution's - this file pins
// them to the shapes the composed arm has to
// answer without a single connected component to walk: isolated vertices and a
// graph that is bipartite only because its components are considered separately.
public sealed partial class IsGraphBipartiteSolutionTests
{
    public static TheoryData<BipartiteCase> Cases =>
        new()
        {
            // A single vertex with no edges.
            { new BipartiteCase(Graph: [[]], Expected: true) },

            // Two isolated vertices - two components, neither with an edge.
            { new BipartiteCase(Graph: [[], []], Expected: true) },

            // Two disjoint edges: bipartite across more than one component.
            { new BipartiteCase(Graph: [[1], [0], [3], [2]], Expected: true) },

            // A triangle is the smallest odd cycle.
            { new BipartiteCase(Graph: [[1, 2], [0, 2], [0, 1]], Expected: false) },

            // A star: every edge crosses from the center to a leaf.
            { new BipartiteCase(Graph: [[1, 2], [0], [0]], Expected: true) },

            // One bipartite component and one odd-cycle component.
            { new BipartiteCase(Graph: [[1], [0], [3, 4], [2, 4], [2, 3]], Expected: false) },
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void IsBipartiteByColorArrayDfs_Cases_ReturnsWhetherGraphIsTwoColorable(
        BipartiteCase testCase)
    {
        var actual = IsGraphBipartiteSolution.IsBipartiteByColorArrayDfs(testCase.Graph);

        Assert.Equal(testCase.Expected, actual);
    }

    [Theory]
    [MemberData(nameof(Cases))]
    public void IsBipartiteByBipartiteCheck_Cases_ReturnsWhetherGraphIsTwoColorable(
        BipartiteCase testCase)
    {
        var actual = IsGraphBipartiteSolution.IsBipartiteByBipartiteCheck(testCase.Graph);

        Assert.Equal(testCase.Expected, actual);
    }

    // One case: the adjacency list, and whether the graph is two-colorable.
    // The `bool` is the expected answer rather than a mode, so the row names it instead
    // of leaving a bare `true` in a position the reader has to decode.
    public readonly record struct BipartiteCase(int[][] Graph, bool Expected);
}
