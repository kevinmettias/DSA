namespace DSAExperimentation.LeetCode.Solutions.CherryPickup;

// The reach of CherryPickupSolution's strategies (ARCHITECTURE 17.13): the catalog cases every one of them
// finishes in the harness's time. MaxCherriesByUnmemoizedRecursion walks both trips' every pair of paths, and
// takes 81.8 ms over the 60 of the catalog's 77 cases with up to 6 rows (measured 2026-11-05, Release); a
// 7-row grid takes it up to 47 ms alone, an 8-row one 452 ms, and both 9-row grids run past 1.5 s.
internal static class CherryPickupReach
{
    private const int LargestRowCount = 6;

    public static bool IsWithin(int[,] grid) =>
        grid.GetLength(0) <= LargestRowCount;
}
