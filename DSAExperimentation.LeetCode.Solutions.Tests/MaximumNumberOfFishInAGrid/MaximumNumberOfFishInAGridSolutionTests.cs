using DSAExperimentation.LeetCode.Solutions.MaximumNumberOfFishInAGrid;

namespace DSAExperimentation.LeetCode.Solutions.Tests.MaximumNumberOfFishInAGrid;

// Harness only. Both strategies are MaximumNumberOfFishInAGridSolution's -
// including the recursive flood fill, which the benchmark used to own privately as
// its baseline and nothing asserted. The cases pin what makes this problem
// different from the binary-grid flood fills: the score is a SUM of cell values
// rather than a cell count, so a grid whose largest component is not its heaviest
// one catches a strategy that kept counting cells, and a grid whose best component
// comes last catches one that returned the first component it found. LeetCode's
// published examples run in CatalogCaseTests.
public sealed partial class MaximumNumberOfFishInAGridSolutionTests
{
    public static TheoryData<int[][], int> Cases =>
        new()
        {
            // No water at all - there is nowhere to start, so the answer is 0.
            { [[0, 0], [0, 0]], 0 },

            // A single water cell is its own component.
            { [[5]], 5 },

            // The two-cell component sums to 2; the single heavy cell later in the
            // sweep is worth 9. Cell COUNT would pick the wrong one, and stopping
            // at the first component found would too.
            { [[1, 1, 0, 0], [0, 0, 0, 9]], 9 },
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void MaxFishByNaiveFloodFill_Cases_ReturnsLargestComponentFishTotal(
        int[][] grid, int expected) =>
        Assert.Equal(expected, MaximumNumberOfFishInAGridSolution.MaxFishByNaiveFloodFill(grid));

    [Theory]
    [MemberData(nameof(Cases))]
    public void MaxFishByDepthFirstSearch_Cases_ReturnsLargestComponentFishTotal(
        int[][] grid, int expected) =>
        Assert.Equal(expected, MaximumNumberOfFishInAGridSolution.MaxFishByDepthFirstSearch(grid));
}
