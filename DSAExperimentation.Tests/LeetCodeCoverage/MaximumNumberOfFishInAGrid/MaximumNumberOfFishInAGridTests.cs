using DSAExperimentation.LeetCode.MaximumNumberOfFishInAGrid;

namespace DSAExperimentation.Tests.LeetCodeCoverage.MaximumNumberOfFishInAGrid;

// Harness only. Both strategies are MaximumNumberOfFishInAGridSolution's -
// including the recursive flood fill, which the benchmark used to own privately as
// its baseline and nothing asserted. Beyond LeetCode's two published examples the
// cases pin what makes this problem different from the binary-grid flood fills:
// the score is a SUM of cell values rather than a cell count, so a grid whose
// largest component is not its heaviest one catches a strategy that kept counting
// cells, and a grid whose best component comes last catches one that returned the
// first component it found.
public sealed class MaximumNumberOfFishInAGridTests
{
    public static TheoryData<int[][], int> Examples =>
        new()
        {
            // LC example 1: the (1,3)-(2,3) pair sums to 7, beating every other
            // water component.
            {
                [
                    [0, 2, 1, 0],
                    [4, 0, 0, 3],
                    [1, 0, 0, 4],
                    [0, 3, 2, 0],
                ],
                7
            },

            // LC example 2: two isolated single-cell components, both worth 1.
            {
                [
                    [1, 0, 0, 0],
                    [0, 0, 0, 0],
                    [0, 0, 0, 0],
                    [0, 0, 0, 1],
                ],
                1
            },

            // No water at all - there is nowhere to start, so the answer is 0.
            { [[0, 0], [0, 0]], 0 },

            // A single water cell is its own component.
            { [[5]], 5 },

            // Every cell is water and connected, so the whole grid is one component.
            { [[1, 2], [3, 4]], 10 },

            // The two-cell component sums to 2; the single heavy cell later in the
            // sweep is worth 9. Cell COUNT would pick the wrong one, and stopping
            // at the first component found would too.
            { [[1, 1, 0, 0], [0, 0, 0, 9]], 9 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void MaxFishByNaiveFloodFill_LeetCodeExamples_ReturnsLargestComponentFishTotal(
        int[][] grid, int expected) =>
        Assert.Equal(expected, MaximumNumberOfFishInAGridSolution.MaxFishByNaiveFloodFill(grid));

    [Theory]
    [MemberData(nameof(Examples))]
    public void MaxFishByDepthFirstSearch_LeetCodeExamples_ReturnsLargestComponentFishTotal(
        int[][] grid, int expected) =>
        Assert.Equal(expected, MaximumNumberOfFishInAGridSolution.MaxFishByDepthFirstSearch(grid));
}
