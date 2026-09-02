using DSAExperimentation.LeetCode.MinimumTimeToVisitACellInAGrid;

namespace DSAExperimentation.Tests.LeetCodeCoverage.MinimumTimeToVisitACellInAGrid;

// Harness only: the algorithms live in MinimumTimeToVisitACellInAGridSolution. One
// test method per strategy over one shared set of LeetCode's own examples, so a
// failure names the strategy that broke (TwoSumTests precedent).
public sealed class MinimumTimeToVisitACellInAGridTests
{
    public static TheoryData<int[][], int> Examples =>
        new()
        {
            { [[0, 1, 3, 2], [5, 1, 2, 5], [4, 3, 8, 6]], 7 },
            { [[0, 2, 4], [3, 2, 1], [1, 0, 4]], -1 }, // grid[0][1]=2 and grid[1][0]=3: no first move exists
            { [[0]], 0 }, // already at the target
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void MinimumTimeByBclPriorityQueue_LeetCodeExamples_ReturnsMinimumArrivalTime(int[][] grid, int expected)
    {
        var actual = MinimumTimeToVisitACellInAGridSolution.MinimumTimeByBclPriorityQueue(grid);
        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void MinimumTimeByHeap_LeetCodeExamples_ReturnsMinimumArrivalTime(int[][] grid, int expected)
    {
        var actual = MinimumTimeToVisitACellInAGridSolution.MinimumTimeByHeap(grid);
        Assert.Equal(expected, actual);
    }
}
