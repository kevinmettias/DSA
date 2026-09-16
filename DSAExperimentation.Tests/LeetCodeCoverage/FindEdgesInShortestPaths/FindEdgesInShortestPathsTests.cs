using DSAExperimentation.LeetCode.FindEdgesInShortestPaths;

namespace DSAExperimentation.Tests.LeetCodeCoverage.FindEdgesInShortestPaths;

// Harness only. The weighted graph itself is EdgeGraph and both Dijkstra strategies
// are FindEdgesInShortestPathsSolution's - this file just pins them to LeetCode's
// published examples.
public sealed class FindEdgesInShortestPathsTests
{
    public static TheoryData<int, int[][], bool[]> Examples =>
        new()
        {
            {
                6,
                [[0, 1, 4], [0, 2, 1], [1, 3, 2], [1, 4, 3], [1, 5, 1], [2, 3, 1], [3, 5, 3], [4, 5, 2]],
                [true, true, true, false, true, true, true, false]
            },
            {
                4,
                [[2, 0, 1], [0, 1, 1], [0, 3, 4], [3, 2, 2]],
                [true, false, false, true]
            },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void AnswerByBruteForceDijkstra_LeetCodeExamples_FlagsEveryShortestPathEdge(
        int n, int[][] edges, bool[] expected)
    {
        var actual = FindEdgesInShortestPathsSolution.AnswerByBruteForceDijkstra(n, edges);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void AnswerByShortestPathDijkstra_LeetCodeExamples_FlagsEveryShortestPathEdge(
        int n, int[][] edges, bool[] expected)
    {
        var actual = FindEdgesInShortestPathsSolution.AnswerByShortestPathDijkstra(n, edges);

        Assert.Equal(expected, actual);
    }
}
