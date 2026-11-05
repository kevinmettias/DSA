using DSAExperimentation.LeetCode.Solutions.NumberOfEnclaves;

namespace DSAExperimentation.LeetCode.Solutions.Tests.NumberOfEnclaves;

// Harness only. Both strategies are NumberOfEnclavesSolution's - this file just
// pins them to grids with no enclave: land in pieces that each touch the border, a
// grid that is all land, and a single water cell. Each strategy clones the grid it
// is handed, so the two theories can share one case set without the first run
// sinking the land the second one needs. LeetCode's published examples run in
// CatalogCaseTests.
public sealed partial class NumberOfEnclavesSolutionTests
{
    public static TheoryData<int[][], int> Cases =>
        new()
        {
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
            { [[0]], 0 },
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void CountEnclavesByNaiveFloodFill_Cases_ReturnsLandThatCannotReachTheBorder(
        int[][] grid, int expected) =>
        Assert.Equal(expected, NumberOfEnclavesSolution.CountEnclavesByNaiveFloodFill(grid));

    [Theory]
    [MemberData(nameof(Cases))]
    public void CountEnclavesByDepthFirstSearch_Cases_ReturnsLandThatCannotReachTheBorder(
        int[][] grid, int expected) =>
        Assert.Equal(expected, NumberOfEnclavesSolution.CountEnclavesByDepthFirstSearch(grid));
}
