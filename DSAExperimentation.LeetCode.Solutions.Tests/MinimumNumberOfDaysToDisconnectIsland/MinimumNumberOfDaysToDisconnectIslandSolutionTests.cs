using DSAExperimentation.LeetCode.Solutions.MinimumNumberOfDaysToDisconnectIsland;

namespace DSAExperimentation.LeetCode.Solutions.Tests.MinimumNumberOfDaysToDisconnectIsland;

// Harness only. Both strategies are MinimumNumberOfDaysToDisconnectIslandSolution's -
// MinDaysByNaiveFloodFill (previously untested scaffolding inlined in the benchmark
// as its baseline arm) now gets the same cases as MinDaysByDepthFirstSearch
// (previously the test's own private helper), so a failure names the strategy that
// broke. The cases are grids that are already disconnected or hold no land, which
// take no days, and a single cell and a three-cell row, which take one day each.
// LeetCode's published examples run in CatalogCaseTests.
//
// The single-land-cell case changed answer on purpose: the pre-migration test
// short-circuited "one or fewer land cells" to 0, but a grid with exactly one island
// is connected by LC 1568's own definition, so [[1]] takes one day - remove that cell
// and zero islands remain. The benchmark arm, which had no such short-circuit, was
// the one that agreed with LeetCode.
public sealed partial class MinimumNumberOfDaysToDisconnectIslandSolutionTests
{
    public static TheoryData<int[][], int> Cases =>
        new()
        {
            { [[1, 0], [0, 1]], 0 },
            { [[0, 0], [0, 0]], 0 },
            { [[1]], 1 },
            { [[1, 1, 1]], 1 },
            { [[1, 1, 0, 1, 1]], 0 },
            { [[1, 0, 1, 0], [0, 1, 0, 1]], 0 },
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void MinDaysByNaiveFloodFill_Cases_ReturnsDaysToDisconnect(
        int[][] grid, int expected) =>
        Assert.Equal(expected, MinimumNumberOfDaysToDisconnectIslandSolution.MinDaysByNaiveFloodFill(grid));

    [Theory]
    [MemberData(nameof(Cases))]
    public void MinDaysByDepthFirstSearch_Cases_ReturnsDaysToDisconnect(
        int[][] grid, int expected) =>
        Assert.Equal(expected, MinimumNumberOfDaysToDisconnectIslandSolution.MinDaysByDepthFirstSearch(grid));
}
