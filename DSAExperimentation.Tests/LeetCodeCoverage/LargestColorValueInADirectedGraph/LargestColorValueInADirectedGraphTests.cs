using DSAExperimentation.LeetCode.LargestColorValueInADirectedGraph;

namespace DSAExperimentation.Tests.LeetCodeCoverage.LargestColorValueInADirectedGraph;

// Harness only. Both strategies are LargestColorValueInADirectedGraphSolution's -
// this file pins them to LeetCode's published examples in LeetCode's own
// (colors, edges) input shape, plus the cases the two arms have to agree on: an
// edgeless graph, a path whose colors are all distinct, a path whose answer is not
// its final node's color, and a cycle that is longer than the self-loop LeetCode's
// own second example uses.
public sealed class LargestColorValueInADirectedGraphTests
{
    public static TheoryData<string, int[][], int> Examples =>
        new()
        {
            { "abaca", [[0, 1], [0, 2], [2, 3], [3, 4]], 3 },
            { "a", [[0, 0]], -1 },
            { "aaa", [], 1 },
            { "abc", [[0, 1], [1, 2]], 1 },
            { "aabbb", [[0, 1], [1, 2], [2, 3], [3, 4]], 3 },
            { "abc", [[0, 1], [1, 2], [2, 0]], -1 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void LargestPathValueByKahnsTopologicalSort_LeetCodeExamples_ReturnsLargestColorCountOnAnyPath(
        string colors, int[][] edges, int expected) =>
        Assert.Equal(
            expected,
            LargestColorValueInADirectedGraphSolution.LargestPathValueByKahnsTopologicalSort(colors, edges));

    [Theory]
    [MemberData(nameof(Examples))]
    public void LargestPathValueByRepeatedRelaxation_LeetCodeExamples_ReturnsLargestColorCountOnAnyPath(
        string colors, int[][] edges, int expected) =>
        Assert.Equal(
            expected,
            LargestColorValueInADirectedGraphSolution.LargestPathValueByRepeatedRelaxation(colors, edges));
}
