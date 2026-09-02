using DSAExperimentation.LeetCode.FindASafeWalkThroughAGrid;

namespace DSAExperimentation.Tests.LeetCodeCoverage.FindASafeWalkThroughAGrid;

// Harness only: both strategies are FindASafeWalkThroughAGridSolution's - this
// file just pins them to LeetCode's published examples, including the one where
// every path except a single detour is unsafe.
public sealed class FindASafeWalkThroughAGridTests
{
    public static TheoryData<int[][], int, bool> Examples =>
        new()
        {
            { [[0, 1, 0, 0, 0], [0, 1, 0, 1, 0], [0, 0, 0, 1, 0]], 1, true },
            { [[0, 1, 1, 0, 0, 0], [1, 0, 1, 0, 0, 0], [0, 1, 1, 1, 0, 1], [0, 0, 1, 0, 1, 0]], 3, false },
            { [[1, 1, 1], [1, 0, 1], [1, 1, 1]], 5, true },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void IsSafeByBruteForceArrayDijkstra_LeetCodeExamples_ReturnsWhetherFinalHealthStaysPositive(
        int[][] grid, int health, bool expected) =>
        Assert.Equal(expected, FindASafeWalkThroughAGridSolution.IsSafeByBruteForceArrayDijkstra(grid, health));

    [Theory]
    [MemberData(nameof(Examples))]
    public void IsSafeByWeightedGridDijkstra_LeetCodeExamples_ReturnsWhetherFinalHealthStaysPositive(
        int[][] grid, int health, bool expected) =>
        Assert.Equal(expected, FindASafeWalkThroughAGridSolution.IsSafeByWeightedGridDijkstra(grid, health));
}
