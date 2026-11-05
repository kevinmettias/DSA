namespace DSAExperimentation.LeetCode.Solutions.CyclicallyRotatingAGrid;

// The reach of CyclicallyRotatingAGridSolution's strategies (ARCHITECTURE 17.13): the catalog cases every one
// of them finishes in the harness's time. RotateGridByStepwiseQueue takes 3.2 ms over the 16 of the catalog's
// 51 cases with a length of grid up to 5 and a value of rotationSteps up to 50 (measured 2026-11-05,
// Release); the larger cases run past the 250 ms a strategy's catalog cases are allowed.
internal static class CyclicallyRotatingAGridReach
{
    private const int LargestGridLength = 5;
    private const int LargestRotationSteps = 50;

    public static bool IsWithin(int[][] grid, int rotationSteps) =>
        grid.Length <= LargestGridLength && rotationSteps <= LargestRotationSteps;
}
