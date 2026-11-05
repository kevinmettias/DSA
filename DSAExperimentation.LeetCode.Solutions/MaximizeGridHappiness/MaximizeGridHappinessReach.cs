namespace DSAExperimentation.LeetCode.Solutions.MaximizeGridHappiness;

// The reach of MaximizeGridHappinessSolution's strategies (ARCHITECTURE 17.13): the catalog cases every one
// of them finishes in the harness's time. GetMaxGridHappinessByBruteForceRecursion takes 159.1 ms over the 82
// of the catalog's 102 cases with a value of introvertsCount up to 4 (measured 2026-11-05, Release); the
// larger cases run past the 250 ms a strategy's catalog cases are allowed.
internal static class MaximizeGridHappinessReach
{
    private const int LargestIntrovertsCount = 4;

    public static bool IsWithin(int rowCount, int columnCount, int introvertsCount, int extrovertsCount) =>
        introvertsCount <= LargestIntrovertsCount;
}
