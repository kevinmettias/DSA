using DSAExperimentation.LeetCode.NumberOfPossibleSetsOfClosingBranches;

namespace DSAExperimentation.Tests.LeetCodeCoverage.NumberOfPossibleSetsOfClosingBranches;

// Harness only: both strategies live in
// NumberOfPossibleSetsOfClosingBranchesSolution. One test method per strategy
// over one shared set of LeetCode's published examples, so a failure names the
// strategy that broke.
public sealed class NumberOfPossibleSetsOfClosingBranchesTests
{
    public static TheoryData<int, int[][], int, long> Examples =>
        new()
        {
            { 3, [[0, 1, 2], [1, 2, 10], [0, 2, 10]], 5, 5 },
            { 1, [], 10, 2 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void CountClosingSetsByBruteForceFloydWarshall_LeetCodeExamples_ReturnsPossibleClosingSetCount(
        int n, int[][] roads, int maxDistance, long expected)
    {
        var actual =
            NumberOfPossibleSetsOfClosingBranchesSolution.CountClosingSetsByBruteForceFloydWarshall(n, roads, maxDistance);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void CountClosingSetsByAllPairsShortestPaths_LeetCodeExamples_ReturnsPossibleClosingSetCount(
        int n, int[][] roads, int maxDistance, long expected)
    {
        var actual =
            NumberOfPossibleSetsOfClosingBranchesSolution.CountClosingSetsByAllPairsShortestPaths(n, roads, maxDistance);

        Assert.Equal(expected, actual);
    }
}
