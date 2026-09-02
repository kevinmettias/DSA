using DSAExperimentation.LeetCode.MinimumEdgeTogglesOnATree;

namespace DSAExperimentation.Tests.LeetCodeCoverage.MinimumEdgeTogglesOnATree;

// Harness only. The rooted tree itself is LeetCode.MinimumEdgeTogglesOnATree's
// ToggleTree and both strategies are MinimumEdgeTogglesOnATreeSolution's - this
// file just pins them to LeetCode's published examples, including the
// unsatisfiable case that has to answer [-1] without ever choosing an edge.
public sealed class MinimumEdgeTogglesOnATreeTests
{
    public static TheoryData<int, int[][], string, string, int[]> Examples =>
        new()
        {
            { 3, [[0, 1], [1, 2]], "010", "100", [0] },
            { 7, [[0, 1], [1, 2], [2, 3], [3, 4], [3, 5], [1, 6]], "0011000", "0010001", [1, 2, 5] },
            { 2, [[0, 1]], "00", "01", [-1] },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void MinTogglesByBruteForceDfs_LeetCodeExamples_ReturnsSortedEdgeIndices(
        int n, int[][] edges, string start, string target, int[] expected) =>
        Assert.Equal(expected, MinimumEdgeTogglesOnATreeSolution.MinTogglesByBruteForceDfs(n, edges, start, target));

    [Theory]
    [MemberData(nameof(Examples))]
    public void MinTogglesByTreeFold_LeetCodeExamples_ReturnsSortedEdgeIndices(
        int n, int[][] edges, string start, string target, int[] expected) =>
        Assert.Equal(expected, MinimumEdgeTogglesOnATreeSolution.MinTogglesByTreeFold(n, edges, start, target));
}
