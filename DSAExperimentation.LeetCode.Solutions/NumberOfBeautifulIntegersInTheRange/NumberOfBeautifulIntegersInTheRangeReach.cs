namespace DSAExperimentation.LeetCode.Solutions.NumberOfBeautifulIntegersInTheRange;

// The reach of NumberOfBeautifulIntegersInTheRangeSolution's strategies (ARCHITECTURE 17.13): the catalog
// cases every one of them finishes in the harness's time. CountByBruteForce takes 249.3 ms over the 75 of the
// catalog's 97 cases with a value of high up to 9,876,543 (measured 2026-11-05, Release); the larger cases
// run past the 250 ms a strategy's catalog cases are allowed.
internal static class NumberOfBeautifulIntegersInTheRangeReach
{
    private const int LargestHigh = 9_876_543;

    public static bool IsWithin(int low, int high, int divisor) =>
        high <= LargestHigh;
}
