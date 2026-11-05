namespace DSAExperimentation.LeetCode.Solutions.CountAlmostEqualPairsII;

// The reach of CountAlmostEqualPairsIISolution's strategies (ARCHITECTURE 17.13): the catalog cases every one
// of them finishes in the harness's time. CountByBoundedSwapBacktrack takes 142.8 ms over the 123 of the
// catalog's 123 cases with a length of nums up to 21 (measured 2026-11-05, Release); the larger cases run
// past the 250 ms a strategy's catalog cases are allowed.
internal static class CountAlmostEqualPairsIIReach
{
    private const int LargestNumsLength = 21;

    public static bool IsWithin(int[] nums) =>
        nums.Length <= LargestNumsLength;
}
