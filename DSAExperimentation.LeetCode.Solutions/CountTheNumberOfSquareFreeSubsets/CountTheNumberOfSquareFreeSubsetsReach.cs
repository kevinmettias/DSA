namespace DSAExperimentation.LeetCode.Solutions.CountTheNumberOfSquareFreeSubsets;

// The reach of CountTheNumberOfSquareFreeSubsetsSolution's strategies (ARCHITECTURE 17.13): the catalog cases
// every one of them finishes in the harness's time. CountByBruteForce takes 245.4 ms over the 30 of the
// catalog's 55 cases with a length of nums up to 21 (measured 2026-11-05, Release); the larger cases run past
// the 250 ms a strategy's catalog cases are allowed.
internal static class CountTheNumberOfSquareFreeSubsetsReach
{
    private const int LargestNumsLength = 21;

    public static bool IsWithin(int[] nums) =>
        nums.Length <= LargestNumsLength;
}
