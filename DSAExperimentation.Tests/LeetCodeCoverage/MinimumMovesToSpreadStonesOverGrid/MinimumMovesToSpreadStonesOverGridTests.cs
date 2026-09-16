using DSAExperimentation.LeetCode.MinimumMovesToSpreadStonesOverGrid;

namespace DSAExperimentation.Tests.LeetCodeCoverage.MinimumMovesToSpreadStonesOverGrid;

// Harness only: the algorithms live in
// MinimumMovesToSpreadStonesOverGridSolution. One test method per strategy over
// one shared set of LeetCode's own examples, so a failure names the strategy that
// broke.
public sealed partial class MinimumMovesToSpreadStonesOverGridTests
{
    public static TheoryData<int[][], int> Examples =>
        new()
        {
            { [[1, 1, 0], [1, 1, 1], [1, 2, 1]], 3 },
            { [[1, 3, 0], [1, 0, 0], [1, 0, 3]], 4 },
            { [[1, 1, 1], [1, 1, 1], [1, 1, 1]], 0 }, // already balanced, no moves needed
        };

    public static TheoryData<int[]> RandomizedGrids =>
        new() { new[] { 9, 0, 0, 0, 0, 0, 0, 0, 0 }, new[] { 0, 0, 4, 0, 3, 0, 2, 0, 0 }, new[] { 2, 2, 1, 0, 1, 0, 1, 1, 1 } };

    [Theory]
    [MemberData(nameof(Examples))]
    public void MinimumMovesByBruteForcePermutation_LeetCodeExamples_ReturnsMinimumMoves(int[][] grid, int expected)
        => Assert.Equal(expected, MinimumMovesToSpreadStonesOverGridSolution.MinimumMovesByBruteForcePermutation(grid));

    [Theory]
    [MemberData(nameof(Examples))]
    public void MinimumMovesByBacktrackPermutation_LeetCodeExamples_ReturnsMinimumMoves(int[][] grid, int expected)
        => Assert.Equal(expected, MinimumMovesToSpreadStonesOverGridSolution.MinimumMovesByBacktrackPermutation(grid));

    [Theory]
    [MemberData(nameof(RandomizedGrids))]
    public void BothStrategies_RandomizedGrids_Agree(int[] flatGrid)
    {
        var grid = new[]
        {
            new[] { flatGrid[0], flatGrid[1], flatGrid[2] },
            new[] { flatGrid[3], flatGrid[4], flatGrid[5] },
            new[] { flatGrid[6], flatGrid[7], flatGrid[8] },
        };

        var bruteForce = MinimumMovesToSpreadStonesOverGridSolution.MinimumMovesByBruteForcePermutation(grid);
        var backtrack = MinimumMovesToSpreadStonesOverGridSolution.MinimumMovesByBacktrackPermutation(grid);

        Assert.Equal(bruteForce, backtrack);
    }
}
