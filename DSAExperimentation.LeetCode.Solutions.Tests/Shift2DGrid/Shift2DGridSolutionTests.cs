using DSAExperimentation.LeetCode.Solutions.Shift2DGrid;

namespace DSAExperimentation.LeetCode.Solutions.Tests.Shift2DGrid;

// Harness only: both strategies live in Shift2DGridSolution and are asserted
// against the same cases - including the degenerate shifts (shiftCount = 0 and a
// whole number of cycles) and the single-row/single-column grids, where a row-major
// rotation is easiest to get wrong.
public sealed partial class Shift2DGridSolutionTests
{
    public static TheoryData<int[][], int, int[][]> Cases =>
        new()
        {
            // shiftCount larger than the cell count wraps around: 6 mod 4 = 2.
            { [[1, 2], [3, 4]], 6, [[3, 4], [1, 2]] },

            // No shift at all.
            { [[1, 2], [3, 4]], 0, [[1, 2], [3, 4]] },

            // A single row is a plain circular rotation.
            { [[1, 2, 3, 4]], 2, [[3, 4, 1, 2]] },

            // A single column shifts every value down one row.
            { [[1], [2], [3]], 1, [[3], [1], [2]] },
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void ShiftGridByIndexArithmetic_Cases_ShiftsCellsInRowMajorOrder(
        int[][] grid,
        int shiftCount,
        int[][] expected)
    {
        var actual = Shift2DGridSolution.ShiftGridByIndexArithmetic(grid, shiftCount);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Cases))]
    public void ShiftGridByDequeRotation_Cases_ShiftsCellsInRowMajorOrder(
        int[][] grid,
        int shiftCount,
        int[][] expected)
    {
        var actual = Shift2DGridSolution.ShiftGridByDequeRotation(grid, shiftCount);

        Assert.Equal(expected, actual);
    }
}
