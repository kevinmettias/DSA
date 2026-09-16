using DSAExperimentation.LeetCode.CyclicallyRotatingAGrid;

namespace DSAExperimentation.Tests.LeetCodeCoverage.CyclicallyRotatingAGrid;

// Harness only. Both rotations are CyclicallyRotatingAGridSolution's - LeetCode's
// two published examples, the odd-sided grid whose centre cell has no ring, a
// rotation amount larger than the ring it turns, a rotation amount that is a whole
// number of ring lengths and so changes nothing, and a non-square grid whose single
// ring spans both rows.
public sealed class CyclicallyRotatingAGridTests
{
    public static TheoryData<int[][], int, int[][]> Examples =>
        new()
        {
            { [[40, 10], [30, 20]], 1, [[10, 20], [40, 30]] },
            {
                [[1, 2, 3, 4], [5, 6, 7, 8], [9, 10, 11, 12], [13, 14, 15, 16]],
                2,
                [[3, 4, 8, 12], [2, 11, 10, 16], [1, 7, 6, 15], [5, 9, 13, 14]]
            },
            { [[1, 2, 3], [4, 5, 6], [7, 8, 9]], 2, [[3, 6, 9], [2, 5, 8], [1, 4, 7]] },
            { [[40, 10], [30, 20]], 5, [[10, 20], [40, 30]] },
            { [[1, 2, 3], [4, 5, 6], [7, 8, 9]], 8, [[1, 2, 3], [4, 5, 6], [7, 8, 9]] },
            { [[1, 2, 3, 4], [5, 6, 7, 8]], 1, [[2, 3, 4, 8], [1, 5, 6, 7]] },
        };

    [Theory]
    [MemberData(nameof(Examples))]
    public void RotateGridByStepwiseQueue_LeetCodeExamples_RotatesEveryRingCounterClockwise(
        int[][] grid, int rotationSteps, int[][] expected)
    {
        var rotated = CyclicallyRotatingAGridSolution.RotateGridByStepwiseQueue(grid, rotationSteps);
        Assert.Equal(expected, rotated);
    }

    [Theory]
    [MemberData(nameof(Examples))]
    public void RotateGridByDequeRings_LeetCodeExamples_RotatesEveryRingCounterClockwise(
        int[][] grid, int rotationSteps, int[][] expected)
    {
        var rotated = CyclicallyRotatingAGridSolution.RotateGridByDequeRings(grid, rotationSteps);
        Assert.Equal(expected, rotated);
    }
}
