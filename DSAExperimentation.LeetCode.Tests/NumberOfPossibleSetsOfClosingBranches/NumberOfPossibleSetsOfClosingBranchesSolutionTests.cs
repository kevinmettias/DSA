using DSAExperimentation.LeetCode.NumberOfPossibleSetsOfClosingBranches;

namespace DSAExperimentation.LeetCode.Tests.NumberOfPossibleSetsOfClosingBranches;

// Harness only: both strategies live in
// NumberOfPossibleSetsOfClosingBranchesSolution. One test method per strategy
// over one shared set of LeetCode's published examples, so a failure names the
// strategy that broke. The base distance matrix the brute-force strategy is handed is
// asserted on its own, entry by entry.
public sealed partial class NumberOfPossibleSetsOfClosingBranchesSolutionTests
{
    public static TheoryData<int, int[][], int, long> Examples =>
        new()
        {
            // LeetCode examples 1-3, as (n, roads, maxDistance, expected).
            { 3, [[0, 1, 2], [1, 2, 10], [0, 2, 10]], 5, 5 },
            { 3, [[0, 1, 20], [0, 1, 10], [1, 2, 2], [0, 2, 2]], 5, 7 },
            { 1, [], 10, 2 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void CountClosingSetsByBruteForceFloydWarshall_LeetCodeExamples_ReturnsPossibleClosingSetCount(
        int branchCount, int[][] roads, int maxDistance, long expected)
    {
        var actual =
            NumberOfPossibleSetsOfClosingBranchesSolution.CountClosingSetsByBruteForceFloydWarshall(branchCount, roads, maxDistance);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void CountClosingSetsByAllPairsShortestPaths_LeetCodeExamples_ReturnsPossibleClosingSetCount(
        int branchCount, int[][] roads, int maxDistance, long expected)
    {
        var actual =
            NumberOfPossibleSetsOfClosingBranchesSolution.CountClosingSetsByAllPairsShortestPaths(branchCount, roads, maxDistance);

        Assert.Equal(expected, actual);
    }

    // LeetCode's second example: two parallel roads join 0 and 1, at 20 and 10, so the
    // cheaper 10 stands; 1-2 and 0-2 cost 2 each; every road runs both ways. The matrix
    // holds the roads alone, not shortest paths - 0 to 1 stays 10 though 0 -> 2 -> 1 costs
    // 4 - because which branches may be routed through depends on the open set.
    [Fact]
    public void BuildDistanceMatrix_LeetCodeSecondExample_KeepsTheCheaperParallelRoadInBothDirections()
    {
        var distances = NumberOfPossibleSetsOfClosingBranchesSolution.BuildDistanceMatrix(
            3, [[0, 1, 20], [0, 1, 10], [1, 2, 2], [0, 2, 2]]);
        var rows = RowsOf(distances);

        Assert.Equal([[0L, 10L, 2L], [10L, 0L, 2L], [2L, 2L, 0L]], rows);
    }

    // LeetCode's third example: one branch and no roads, a single zero.
    [Fact]
    public void BuildDistanceMatrix_LeetCodeThirdExample_HoldsOneBranchAtDistanceZero()
    {
        var distances = NumberOfPossibleSetsOfClosingBranchesSolution.BuildDistanceMatrix(1, []);
        var rows = RowsOf(distances);

        Assert.Equal([[0L]], rows);
    }

    private static long[][] RowsOf(long[,] matrix) =>
        [.. Enumerable.Range(0, matrix.GetLength(0)).Select(row => RowOf(matrix, row))];

    private static long[] RowOf(long[,] matrix, int row) =>
        [.. Enumerable.Range(0, matrix.GetLength(1)).Select(col => matrix[row, col])];
}
