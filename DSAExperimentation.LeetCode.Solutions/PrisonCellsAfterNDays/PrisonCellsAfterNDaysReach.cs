namespace DSAExperimentation.LeetCode.Solutions.PrisonCellsAfterNDays;

// The reach of PrisonCellsAfterNDaysSolution's strategies (ARCHITECTURE 17.13): the catalog cases every one
// of them finishes in the harness's time. CellsAfterNDaysByDailySimulation takes 98.5 ms over the 83 of the
// catalog's 100 cases with a value of dayCount up to 1,000,000 (measured 2026-11-05, Release); the larger
// cases run past the 250 ms a strategy's catalog cases are allowed.
internal static class PrisonCellsAfterNDaysReach
{
    private const int LargestDayCount = 1_000_000;

    public static bool IsWithin(int[] cells, int dayCount) =>
        dayCount <= LargestDayCount;
}
