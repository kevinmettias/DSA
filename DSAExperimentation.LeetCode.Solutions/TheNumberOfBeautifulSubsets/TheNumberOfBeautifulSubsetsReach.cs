namespace DSAExperimentation.LeetCode.Solutions.TheNumberOfBeautifulSubsets;

// The reach of TheNumberOfBeautifulSubsetsSolution's strategies (ARCHITECTURE 17.13): the catalog cases every
// one of them finishes in the harness's time. CountBeautifulSubsetsByPrunedBacktracking takes 44.8 ms over
// the 24 of the catalog's 53 cases with a length of nums up to 16 (measured 2026-11-05, Release); the larger
// cases run past the 250 ms a strategy's catalog cases are allowed.
internal static class TheNumberOfBeautifulSubsetsReach
{
    private const int LargestNumsLength = 16;

    public static bool IsWithin(int[] nums, int difference) =>
        nums.Length <= LargestNumsLength;
}
