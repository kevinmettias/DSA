using DSAExperimentation.LeetCode.MinimumTimeForKConnectedComponents;

namespace DSAExperimentation.Tests.LeetCodeCoverage.MinimumTimeForKConnectedComponents;

// Harness only. Both strategies are MinimumTimeForKConnectedComponentsSolution's -
// this file just pins them to LeetCode's published examples, including the
// already-at-k case that needs no edge removed at all (answer 0).
public sealed class MinimumTimeForKConnectedComponentsTests
{
    public static TheoryData<int, int[][], int, int> Examples =>
        new()
        {
            { 2, [[0, 1, 3]], 2, 3 },
            { 3, [[0, 1, 2], [1, 2, 4]], 3, 4 },
            { 3, [[0, 2, 5]], 2, 0 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void MinTimeByBinarySearchUnionFind_LeetCodeExamples_ReturnsMinimumRemovalTime(
        int nodeCount, int[][] edges, int requiredComponents, int expected)
    {
        var actual = MinimumTimeForKConnectedComponentsSolution.MinTimeByBinarySearchUnionFind(
            nodeCount, edges, requiredComponents);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void MinTimeByDescendingUnionFind_LeetCodeExamples_ReturnsMinimumRemovalTime(
        int nodeCount, int[][] edges, int requiredComponents, int expected)
    {
        var actual = MinimumTimeForKConnectedComponentsSolution.MinTimeByDescendingUnionFind(
            nodeCount, edges, requiredComponents);

        Assert.Equal(expected, actual);
    }
}
