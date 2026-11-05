namespace DSAExperimentation.LeetCode.Solutions.PaintingAGridWithThreeDifferentColors;

// The reach of PaintingAGridWithThreeDifferentColorsSolution's strategies (ARCHITECTURE 17.13): the catalog
// cases every one of them finishes in the harness's time. ColorTheGridByBruteForce takes 59.9 ms over the 20
// of the catalog's 61 cases with a value of columns up to 7 (measured 2026-11-05, Release); the larger cases
// run past the 250 ms a strategy's catalog cases are allowed.
internal static class PaintingAGridWithThreeDifferentColorsReach
{
    private const int LargestColumns = 7;

    public static bool IsWithin(int rows, int columns) =>
        columns <= LargestColumns;
}
