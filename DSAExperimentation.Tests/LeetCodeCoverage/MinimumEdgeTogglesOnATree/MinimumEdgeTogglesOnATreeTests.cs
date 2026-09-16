using DSAExperimentation.LeetCode.MinimumEdgeTogglesOnATree;

namespace DSAExperimentation.Tests.LeetCodeCoverage.MinimumEdgeTogglesOnATree;

// Harness only. The rooted tree itself is LeetCode.MinimumEdgeTogglesOnATree's
// ToggleTree and both strategies are MinimumEdgeTogglesOnATreeSolution's - this
// file just pins them to LeetCode's published examples, including the
// unsatisfiable case that has to answer [-1] without ever choosing an edge.
public sealed partial class MinimumEdgeTogglesOnATreeTests
{
    public static TheoryData<ToggleExample> Examples =>
        new()
        {
            { new ToggleExample(N: 3, Edges: [[0, 1], [1, 2]], Start: "010", Target: "100", Expected: [0]) },
            {
                new ToggleExample(
                    N: 7,
                    Edges: [[0, 1], [1, 2], [2, 3], [3, 4], [3, 5], [1, 6]],
                    Start: "0011000",
                    Target: "0010001",
                    Expected: [1, 2, 5])
            },
            { new ToggleExample(N: 2, Edges: [[0, 1]], Start: "00", Target: "01", Expected: [-1]) },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void MinTogglesByBruteForceDfs_LeetCodeExamples_ReturnsSortedEdgeIndices(ToggleExample example)
    {
        var actual = MinimumEdgeTogglesOnATreeSolution.MinTogglesByBruteForceDfs(
            example.N, example.Edges, example.Start, example.Target);

        Assert.Equal(example.Expected, actual);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void MinTogglesByTreeFold_LeetCodeExamples_ReturnsSortedEdgeIndices(ToggleExample example)
    {
        var actual = MinimumEdgeTogglesOnATreeSolution.MinTogglesByTreeFold(
            example.N, example.Edges, example.Start, example.Target);

        Assert.Equal(example.Expected, actual);
    }

    // One LeetCode example: the tree, the two colourings, and the edge indices that
    // have to be toggled. Named fields rather than five positional arguments, so a
    // row states which colouring `Start` is - the two are the same `string` type and
    // a transposition would leave both strategies quietly answering the wrong
    // question.
    public readonly record struct ToggleExample(
        int N,
        int[][] Edges,
        string Start,
        string Target,
        int[] Expected);
}
