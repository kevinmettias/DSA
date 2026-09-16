using DSAExperimentation.LeetCode.NumberOfEnclaves;

namespace DSAExperimentation.Tests.LeetCodeCoverage.NumberOfEnclaves;

// Harness only. Both strategies are NumberOfEnclavesSolution's - this file just
// pins them to LeetCode's published examples. Each strategy clones the grid it is
// handed, so the two theories can share one example set without the first run
// sinking the land the second one needs.
public sealed partial class NumberOfEnclavesTests
{
    public static TheoryData<int[][], int> Examples =>
        new()
        {
            {
                [
                    [0, 0, 0, 0],
                    [1, 0, 1, 0],
                    [0, 1, 1, 0],
                    [0, 0, 0, 0],
                ],
                3
            },
            {
                [
                    [0, 1, 1, 0],
                    [0, 0, 1, 0],
                    [1, 0, 0, 0],
                    [0, 1, 1, 0],
                ],
                0
            },
            {
                [
                    [1, 1, 1],
                    [1, 1, 1],
                    [1, 1, 1],
                ],
                0
            },
            {
                [
                    [0, 0, 0, 0, 0],
                    [0, 1, 1, 1, 0],
                    [0, 1, 0, 1, 0],
                    [0, 1, 1, 1, 0],
                    [0, 0, 0, 0, 0],
                ],
                8
            },
            { [[0]], 0 },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void CountEnclavesByNaiveFloodFill_LeetCodeExamples_ReturnsLandThatCannotReachTheBorder(
        int[][] grid, int expected) =>
        Assert.Equal(expected, NumberOfEnclavesSolution.CountEnclavesByNaiveFloodFill(grid));

    [Theory]
    [MemberData(nameof(Examples))]
    public void CountEnclavesByDepthFirstSearch_LeetCodeExamples_ReturnsLandThatCannotReachTheBorder(
        int[][] grid, int expected) =>
        Assert.Equal(expected, NumberOfEnclavesSolution.CountEnclavesByDepthFirstSearch(grid));
}
