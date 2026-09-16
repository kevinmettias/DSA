using DSAExperimentation.LeetCode.Shift2DGrid;

namespace DSAExperimentation.Tests.LeetCodeCoverage.Shift2DGrid;

// Harness only: both strategies live in Shift2DGridSolution and are asserted
// against the same examples - including the degenerate shifts (k = 0 and a whole
// number of cycles) and the single-row/single-column grids, where a row-major
// rotation is easiest to get wrong.
public sealed class Shift2DGridTests
{
    public static TheoryData<int[][], int, int[][]> Examples =>
        new()
        {
            // LeetCode example 1.
            { [[1, 2, 3], [4, 5, 6], [7, 8, 9]], 1, [[9, 1, 2], [3, 4, 5], [6, 7, 8]] },

            // LeetCode example 2: a shift of exactly one row's width moves every
            // row down one place.
            {
                [[3, 8, 1, 9], [19, 7, 2, 5], [4, 6, 11, 10], [12, 0, 21, 13]],
                4,
                [[12, 0, 21, 13], [3, 8, 1, 9], [19, 7, 2, 5], [4, 6, 11, 10]]
            },

            // LeetCode example 3: a full cycle is the identity.
            { [[1, 2, 3], [4, 5, 6], [7, 8, 9]], 9, [[1, 2, 3], [4, 5, 6], [7, 8, 9]] },

            // k larger than the cell count wraps around: 6 mod 4 = 2.
            { [[1, 2], [3, 4]], 6, [[3, 4], [1, 2]] },

            // No shift at all.
            { [[1, 2], [3, 4]], 0, [[1, 2], [3, 4]] },

            // A single row is a plain circular rotation.
            { [[1, 2, 3, 4]], 2, [[3, 4, 1, 2]] },

            // A single column shifts every value down one row.
            { [[1], [2], [3]], 1, [[3], [1], [2]] },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void ShiftGridByIndexArithmetic_LeetCodeExamples_ShiftsCellsInRowMajorOrder(
        int[][] grid,
        int k,
        int[][] expected)
    {
        var actual = Shift2DGridSolution.ShiftGridByIndexArithmetic(grid, k);

        Assert.Equal(expected, actual);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void ShiftGridByDequeRotation_LeetCodeExamples_ShiftsCellsInRowMajorOrder(
        int[][] grid,
        int k,
        int[][] expected)
    {
        var actual = Shift2DGridSolution.ShiftGridByDequeRotation(grid, k);

        Assert.Equal(expected, actual);
    }
}
