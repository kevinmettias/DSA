using DSAExperimentation.LeetCode.MinimumNumberOfDaysToDisconnectIsland;

namespace DSAExperimentation.Tests.LeetCodeCoverage.MinimumNumberOfDaysToDisconnectIsland;

// Harness only. Both strategies are MinimumNumberOfDaysToDisconnectIslandSolution's -
// MinDaysByNaiveFloodFill (previously untested scaffolding inlined in the benchmark
// as its baseline arm) now gets the same examples as MinDaysByDepthFirstSearch
// (previously the test's own private helper), so a failure names the strategy that
// broke.
//
// The single-land-cell example changed answer on purpose: the pre-migration test
// short-circuited "one or fewer land cells" to 0, but a grid with exactly one island
// is connected by LC 1568's own definition, so [[1]] takes one day - remove that cell
// and zero islands remain. The benchmark arm, which had no such short-circuit, was
// the one that agreed with LeetCode.
public sealed class MinimumNumberOfDaysToDisconnectIslandTests
{
    public static TheoryData<int[][], int> Examples =>
        new()
        {
            { [[0, 1, 1, 0], [0, 1, 1, 0], [0, 0, 0, 0]], 2 },
            { [[1, 1]], 2 },
            { [[1, 0], [0, 1]], 0 },
            { [[0, 0], [0, 0]], 0 },
            { [[1]], 1 },
            { [[1, 1, 1]], 1 },
            { [[1, 1, 0, 1, 1]], 0 },
            { [[1, 1, 1], [1, 0, 1], [1, 1, 1]], 2 },
            { [[1, 0, 1, 0], [0, 1, 0, 1]], 0 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void MinDaysByNaiveFloodFill_LeetCodeExamples_ReturnsDaysToDisconnect(
        int[][] grid, int expected) =>
        Assert.Equal(expected, MinimumNumberOfDaysToDisconnectIslandSolution.MinDaysByNaiveFloodFill(grid));

    [Theory]
    [MemberData(nameof(Examples))]
    public void MinDaysByDepthFirstSearch_LeetCodeExamples_ReturnsDaysToDisconnect(
        int[][] grid, int expected) =>
        Assert.Equal(expected, MinimumNumberOfDaysToDisconnectIslandSolution.MinDaysByDepthFirstSearch(grid));
}
