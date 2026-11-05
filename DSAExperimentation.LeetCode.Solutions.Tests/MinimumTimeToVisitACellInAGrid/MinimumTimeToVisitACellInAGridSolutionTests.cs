using DSAExperimentation.LeetCode.Solutions.MinimumTimeToVisitACellInAGrid;

namespace DSAExperimentation.LeetCode.Solutions.Tests.MinimumTimeToVisitACellInAGrid;

// Harness only: the algorithms live in MinimumTimeToVisitACellInAGridSolution. One
// test method per strategy over one shared case, a one-cell grid whose start is
// already the target, so a failure names the strategy that broke (TwoSumTests
// precedent). LeetCode's published examples run in CatalogCaseTests.
public sealed partial class MinimumTimeToVisitACellInAGridSolutionTests
{
    public static TheoryData<int[][], int> Cases =>
        new()
        {
            { [[0]], 0 }, // already at the target
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void MinimumTimeByBclPriorityQueue_Cases_ReturnsMinimumArrivalTime(int[][] grid, int expected)
    {
        var actual = MinimumTimeToVisitACellInAGridSolution.MinimumTimeByBclPriorityQueue(grid);
        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Cases))]
    public void MinimumTimeByHeap_Cases_ReturnsMinimumArrivalTime(int[][] grid, int expected)
    {
        var actual = MinimumTimeToVisitACellInAGridSolution.MinimumTimeByHeap(grid);
        Assert.Equal(expected, actual);
    }
}
