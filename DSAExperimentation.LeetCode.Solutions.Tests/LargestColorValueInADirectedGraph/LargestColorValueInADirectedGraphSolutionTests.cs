using DSAExperimentation.LeetCode.Solutions.LargestColorValueInADirectedGraph;

namespace DSAExperimentation.LeetCode.Solutions.Tests.LargestColorValueInADirectedGraph;

// Harness only. Both strategies are LargestColorValueInADirectedGraphSolution's -
// this file pins them, in LeetCode's own (colors, edges) input shape, to the cases
// the two arms have to agree on: an edgeless graph, a path whose colors are all
// distinct, a path that repeats two colors and counts the one it repeats more, and a cycle that is
// longer than the self-loop LeetCode's own second example uses. LeetCode's
// published examples run in CatalogCaseTests.
public sealed partial class LargestColorValueInADirectedGraphSolutionTests
{
    public static TheoryData<string, int[][], int> Cases =>
        new()
        {
            { "aaa", [], 1 },
            { "abc", [[0, 1], [1, 2]], 1 },
            { "aabbb", [[0, 1], [1, 2], [2, 3], [3, 4]], 3 },
            { "abc", [[0, 1], [1, 2], [2, 0]], -1 },
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void LargestPathValueByKahnsTopologicalSort_Cases_ReturnsLargestColorCountOnAnyPath(
        string colors, int[][] edges, int expected)
    {
        var actual = LargestColorValueInADirectedGraphSolution.LargestPathValueByKahnsTopologicalSort(colors, edges);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Cases))]
    public void LargestPathValueByRepeatedRelaxation_Cases_ReturnsLargestColorCountOnAnyPath(
        string colors, int[][] edges, int expected)
    {
        var actual = LargestColorValueInADirectedGraphSolution.LargestPathValueByRepeatedRelaxation(colors, edges);

        Assert.Equal(expected, actual);
    }
}
