using DSAExperimentation.LeetCode.Solutions.CyclicallyRotatingAGrid;

namespace DSAExperimentation.LeetCode.Solutions.Tests.CyclicallyRotatingAGrid;

// Harness only. Both rotations are CyclicallyRotatingAGridSolution's - the
// odd-sided grid whose centre cell has no ring, a rotation amount larger than the
// ring it turns, a rotation amount that is a whole number of ring lengths and so
// changes nothing, and a non-square grid whose single ring spans both rows.
// LeetCode's published examples run in CatalogCaseTests.
public sealed partial class CyclicallyRotatingAGridSolutionTests
{
    public static TheoryData<int[][], int, int[][]> Cases =>
        new()
        {
            { [[1, 2, 3], [4, 5, 6], [7, 8, 9]], 2, [[3, 6, 9], [2, 5, 8], [1, 4, 7]] },
            { [[40, 10], [30, 20]], 5, [[10, 20], [40, 30]] },
            { [[1, 2, 3], [4, 5, 6], [7, 8, 9]], 8, [[1, 2, 3], [4, 5, 6], [7, 8, 9]] },
            { [[1, 2, 3, 4], [5, 6, 7, 8]], 1, [[2, 3, 4, 8], [1, 5, 6, 7]] },
        };

    [Theory]
    [MemberData(nameof(Cases))]
    public void RotateGridByStepwiseQueue_Cases_RotatesEveryRingCounterClockwise(
        int[][] grid, int rotationSteps, int[][] expected)
    {
        var rotated = CyclicallyRotatingAGridSolution.RotateGridByStepwiseQueue(grid, rotationSteps);
        Assert.Equal(expected, rotated);
    }

    [Theory]
    [MemberData(nameof(Cases))]
    public void RotateGridByDequeRings_Cases_RotatesEveryRingCounterClockwise(
        int[][] grid, int rotationSteps, int[][] expected)
    {
        var rotated = CyclicallyRotatingAGridSolution.RotateGridByDequeRings(grid, rotationSteps);
        Assert.Equal(expected, rotated);
    }
}
