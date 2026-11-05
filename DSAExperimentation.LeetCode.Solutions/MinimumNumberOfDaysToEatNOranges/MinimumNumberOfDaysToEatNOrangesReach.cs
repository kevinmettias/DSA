namespace DSAExperimentation.LeetCode.Solutions.MinimumNumberOfDaysToEatNOranges;

// The reach of MinimumNumberOfDaysToEatNOrangesSolution's strategies (ARCHITECTURE 17.13): the catalog cases
// every one of them finishes in the harness's time. MinDaysByUnmemoizedRecursion takes 226.1 ms over the 211
// of the catalog's 232 cases with a value of orangeCount up to 666,666,666 (measured 2026-11-05, Release);
// the larger cases run past the 250 ms a strategy's catalog cases are allowed.
internal static class MinimumNumberOfDaysToEatNOrangesReach
{
    private const int LargestOrangeCount = 666_666_666;

    public static bool IsWithin(int orangeCount) =>
        orangeCount <= LargestOrangeCount;
}
