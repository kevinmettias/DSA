namespace DSAExperimentation.LeetCode.Solutions.NumberOfWaysToWearDifferentHatsToEachOther;

// The reach of NumberOfWaysToWearDifferentHatsToEachOtherSolution's strategies (ARCHITECTURE 17.13): the
// catalog cases every one of them finishes in the harness's time. NumberWaysByBruteForceRecursion takes 132.5
// ms over the 45 of the catalog's 71 cases with a length of hats up to 8 and an element count of hats up to
// 50 (measured 2026-11-05, Release); the larger cases run past the 250 ms a strategy's catalog cases are
// allowed.
internal static class NumberOfWaysToWearDifferentHatsToEachOtherReach
{
    private const int LargestHatsLength = 8;
    private const int LargestHatsElements = 50;

    public static bool IsWithin(int[][] hats) =>
        hats.Length <= LargestHatsLength && hats.Sum(row => row.Length) <= LargestHatsElements;
}
