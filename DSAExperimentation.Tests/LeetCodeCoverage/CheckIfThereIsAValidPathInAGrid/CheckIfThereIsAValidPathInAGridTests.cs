using DSAExperimentation.LeetCode.CheckIfThereIsAValidPathInAGrid;

namespace DSAExperimentation.Tests.LeetCodeCoverage.CheckIfThereIsAValidPathInAGrid;

// Harness only. Both strategies are CheckIfThereIsAValidPathInAGridSolution's -
// this file just pins them to LeetCode's published examples plus the corner cases
// the original coverage carried, one theory per strategy so a failure names the
// strategy that broke.
public sealed class CheckIfThereIsAValidPathInAGridTests
{
    public static TheoryData<int[][], bool> Examples =>
        new()
        {
            // LeetCode example 1: down, right, up, right, down through the corner.
            { [[2, 4, 3], [6, 5, 2]], true },

            // LeetCode example 2: two vertical streets that never connect sideways.
            { [[1, 2, 1], [1, 2, 1]], false },

            // LeetCode example 3: the last street opens up and down, not left.
            { [[1, 1, 2]], false },

            // (0,0)-R->(0,1)-D->(1,1)-D->(2,1)-R->(2,2): two turns via a right-down
            // street (3), a straight vertical street (2), then a right-up street (6).
            { [[1, 3, 1], [2, 2, 2], [1, 6, 1]], true },

            // A single row of horizontal streets is one straight corridor.
            { [[1, 1, 1]], true },

            // A one-cell grid: the start already is the corner.
            { [[1]], true },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void HasValidPathByRecursiveDfs_LeetCodeExamples_ReportsWhetherTheCornerIsReachable(
        int[][] grid, bool expected) =>
        Assert.Equal(expected, CheckIfThereIsAValidPathInAGridSolution.HasValidPathByRecursiveDfs(grid));

    [Theory]
    [MemberData(nameof(Examples))]
    public void HasValidPathByDepthFirstTraverse_LeetCodeExamples_ReportsWhetherTheCornerIsReachable(
        int[][] grid, bool expected) =>
        Assert.Equal(expected, CheckIfThereIsAValidPathInAGridSolution.HasValidPathByDepthFirstTraverse(grid));
}
