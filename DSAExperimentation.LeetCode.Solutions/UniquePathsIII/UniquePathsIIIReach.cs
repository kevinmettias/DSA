namespace DSAExperimentation.LeetCode.Solutions.UniquePathsIII;

// The reach of UniquePathsIIISolution's strategies (ARCHITECTURE 17.13): the catalog cases every one of them
// finishes in the harness's time. CountUniquePathsByBacktrackEngine takes 156 ms over the 60 of the catalog's
// 85 cases with an element count of grid up to 28 (measured 2026-11-05, Release); the larger cases run past
// the 250 ms a strategy's catalog cases are allowed.
internal static class UniquePathsIIIReach
{
    private const int LargestGridElements = 28;

    public static bool IsWithin(int[][] grid) =>
        grid.Sum(row => row.Length) <= LargestGridElements;
}
