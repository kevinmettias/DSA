using DSAExperimentation.LeetCode.NumberOfRestrictedPathsFromFirstToLastNode;

namespace DSAExperimentation.Tests.LeetCodeCoverage.NumberOfRestrictedPathsFromFirstToLastNode;

// Harness only. The graph, its distance labelling and both counting strategies are
// NumberOfRestrictedPathsFromFirstToLastNodeSolution's - this file just pins them to
// LeetCode's published examples, including the second one whose only restricted path
// leaves most of the graph unreachable under the distance-decreasing rule.
public sealed class NumberOfRestrictedPathsFromFirstToLastNodeTests
{
    public static TheoryData<int, int[][], long> Examples =>
        new()
        {
            { 5, [[1, 2, 3], [1, 3, 3], [2, 3, 1], [1, 4, 2], [5, 2, 2], [3, 5, 1], [5, 4, 10]], 3 },
            { 7, [[1, 3, 1], [4, 1, 2], [7, 3, 4], [2, 5, 3], [5, 6, 1], [6, 7, 2], [7, 5, 3], [2, 6, 4]], 1 },
            { 2, [[1, 2, 1]], 1 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void CountRestrictedPathsByNaiveDfs_LeetCodeExamples_CountsEveryDistanceDecreasingPath(
        int nodeCount, int[][] edges, long expected)
    {
        var actual = NumberOfRestrictedPathsFromFirstToLastNodeSolution.CountRestrictedPathsByNaiveDfs(
            nodeCount, edges);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void CountRestrictedPathsByDagFold_LeetCodeExamples_CountsEveryDistanceDecreasingPath(
        int nodeCount, int[][] edges, long expected)
    {
        var actual = NumberOfRestrictedPathsFromFirstToLastNodeSolution.CountRestrictedPathsByDagFold(
            nodeCount, edges);

        Assert.Equal(expected, actual);
    }
}
