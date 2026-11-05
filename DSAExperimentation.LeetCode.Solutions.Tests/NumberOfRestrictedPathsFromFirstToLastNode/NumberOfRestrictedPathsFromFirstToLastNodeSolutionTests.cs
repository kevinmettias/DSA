using DSAExperimentation.LeetCode.Solutions.NumberOfRestrictedPathsFromFirstToLastNode;

namespace DSAExperimentation.LeetCode.Solutions.Tests.NumberOfRestrictedPathsFromFirstToLastNode;

// Harness only. The graph, its distance labelling and both counting strategies are
// NumberOfRestrictedPathsFromFirstToLastNodeSolution's - this file just pins them to
// the smallest graph, a single edge joining the first node to the last, whose one
// path is restricted. LeetCode's published examples run in CatalogCaseTests.
public sealed partial class NumberOfRestrictedPathsFromFirstToLastNodeSolutionTests
{
    public static TheoryData<int, int[][], long> Cases =>
        new()
        {
            { 2, [[1, 2, 1]], 1 },
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void CountRestrictedPathsByNaiveDfs_Cases_CountsEveryDistanceDecreasingPath(
        int nodeCount, int[][] edges, long expected)
    {
        var actual = NumberOfRestrictedPathsFromFirstToLastNodeSolution.CountRestrictedPathsByNaiveDfs(
            nodeCount, edges);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Cases))]
    public void CountRestrictedPathsByDagFold_Cases_CountsEveryDistanceDecreasingPath(
        int nodeCount, int[][] edges, long expected)
    {
        var actual = NumberOfRestrictedPathsFromFirstToLastNodeSolution.CountRestrictedPathsByDagFold(
            nodeCount, edges);

        Assert.Equal(expected, actual);
    }
}
