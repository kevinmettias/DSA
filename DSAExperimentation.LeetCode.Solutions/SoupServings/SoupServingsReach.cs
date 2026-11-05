namespace DSAExperimentation.LeetCode.Solutions.SoupServings;

// The reach of SoupServingsSolution's strategies (ARCHITECTURE 17.13): the catalog cases every one of them
// finishes in the harness's time. ProbabilityByUnmemoizedRecursion takes 19.9 ms over the 11 of the catalog's
// 52 cases with a value of milliliters up to 625 (measured 2026-11-05, Release); the larger cases run past
// the 250 ms a strategy's catalog cases are allowed.
internal static class SoupServingsReach
{
    private const int LargestMilliliters = 625;

    public static bool IsWithin(int milliliters) =>
        milliliters <= LargestMilliliters;
}
